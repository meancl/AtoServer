using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.IO.MemoryMappedFiles;
using System.Runtime.InteropServices;
using AtoServer.AI;
using System.Threading;

namespace AtoServer.Shared_Memory
{
    public class MMF
    {
        public const string sMemoryName = "AtoMemory";

        public const int SERVER_POINTER_LOC = 0;
        public const int USER_POINTER_LOC = 4;
        public const int nStepPtrSize = 8; 
        public const int nStructStepNum = 8192;

        public int nStructSize = Marshal.SizeOf<SharedAIBlock>();
        public int nTotalMemorySize;

        public MemoryMappedFile mappedMemory;

        public int nPtrOffSet;
        public int nQueueOffSet;
        public int nBlockOffSet;

        public int nCurMyPtr; // 우리꺼
        public int nCurOtherPtr; // 남의꺼


        public SharedAIBlock curBlock;
        public AIStarter aIStarter;
     
        public MMF()
        {
            nTotalMemorySize = nStepPtrSize + nStructStepNum * sizeof(int) + nStructSize * nStructStepNum;

            nPtrOffSet = 0;
            nQueueOffSet = nPtrOffSet + nStepPtrSize;
            nBlockOffSet = nQueueOffSet + nStructStepNum * sizeof(int);

            Console.WriteLine("AI모듈이 생성 시작합니다.");
            aIStarter = new AIStarter();
            Console.WriteLine("AI모듈이 생성됐습니다.");
            Console.WriteLine("공유메모리 생성 시작합니다.");
            CreateSharedMemory();
            Console.WriteLine("공유메모리 생성됐습니다.");
            ServeAIService();
        }

        ~MMF()
        {
            DisposeSharedMemory();
        }

        // 공유메모리 생성하기
        public void CreateSharedMemory()
        {
            mappedMemory = MemoryMappedFile.CreateNew(sMemoryName, nTotalMemorySize);
            using (var accessor = mappedMemory.CreateViewAccessor())
            {
                // 초기화할 데이터를 포함하는 바이트 배열
                byte[] data = new byte[nTotalMemorySize];
                for (int i = 0; i < data.Length; i++)
                {
                    data[i] = (byte)0;
                }
                accessor.WriteArray(0, data, 0, data.Length);
            }
        }

        // 공유메모리 해제하기
        public void DisposeSharedMemory()
        {
            mappedMemory.Dispose();
        }

        // 내 ptr과 클라이언트 ptr 다른지 체크하기
        bool CheckIsDataComed()
        {
            return (nCurOtherPtr >= nCurMyPtr) ? nCurOtherPtr > nCurMyPtr : nCurMyPtr != nCurOtherPtr;
        }


        
        public const int FAKE_REQUEST_SIGNAL = -1;
        public const int FAKE_BUY_SIGNAL = 0;
        public const int FAKE_RESIST_SIGNAL = 1;
        public const int FAKE_ASSISTANT_SIGNAL = 2;
        public const int REAL_BUY_SIGNAL = 5;
        public const int REAL_SELL_SIGNAL = 6;
        public const int FAKE_VOLATILE_SIGNAL = 7;
        public const int EVERY_SIGNAL = 8;
        public const int FAKE_DOWN_SIGNAL = 9;
        public const int PAPER_BUY_SIGNAL = 10;
        public const int PAPER_SELL_SIGNAL = 11;

        public int nRequestCnt = 0;

        public void ServeAIService()
        {
            /*
             * 1. 데이터를 읽어 nPtr1 과 nPtr2의 차를 구한다
             * 2. 처리해야할 번째 데이터들을 한번에 가져온다
             * 3. 데이터를 계산한다
             * 4. nPtr2 카운트를 한칸 늘리고 남은 데이터가 있다면 3번부터 다시 반복한다
             */
            using (var accessor = mappedMemory.CreateViewAccessor())
            {
                accessor.Read(nPtrOffSet + SERVER_POINTER_LOC, out nCurMyPtr); // 내 ptr받아오기
                accessor.Read(nPtrOffSet + USER_POINTER_LOC, out nCurOtherPtr); // 클라이언트 ptr 받아오기

                while (CheckIsDataComed()) // 내 ptr과 클라이언트 ptr 다른 지 확인하기
                {
                    accessor.Read(nQueueOffSet + nCurMyPtr * sizeof(int), out int nCurIdxPtr); // 위치 인덱스 받아오기
                    accessor.Read(nBlockOffSet + nCurIdxPtr * nStructSize, out curBlock); // 위치인덱스 메모리에서 데이터 꺼내오기

                    Tuple<float, bool> answer;
                    string sReqMsg;

                    switch (curBlock.nRequestType)
                    {
                        case REAL_BUY_SIGNAL:
                            answer = CalculateBuyMLResult();
                            sReqMsg = "실매수";
                            break;
                        case REAL_SELL_SIGNAL:
                            answer = CalculateSellMLResult();
                            sReqMsg = "실매도";
                            break;
                        case FAKE_REQUEST_SIGNAL:
                            answer = CalculateFakeMLResult();
                            sReqMsg = "페이크";
                            break;
                        case EVERY_SIGNAL:
                            answer = CalculateFakeMLResult();
                            sReqMsg = "에브리";
                            break;
                        case PAPER_BUY_SIGNAL:
                            answer = CalculateBuyMLResult();
                            sReqMsg = "모의매수";
                            break;
                        case PAPER_SELL_SIGNAL:
                            answer = CalculateSellMLResult();
                            sReqMsg = "모의매도";
                            break;
                        default:
                            return;
                    }

#if CONSOL
                    string sCode;
                    unsafe
                    {
                        fixed (SharedAIBlock* ptr = &curBlock)
                        {
                            sCode = new string(ptr->cCodeArr, 0, 6);
                        }
                    }
                    Console.WriteLine($"{++nRequestCnt}  servicePtr : {nCurMyPtr}, clientPtr : {nCurOtherPtr}, sCode : {sCode}, nCurIdxPtr : {nCurIdxPtr}, reqTime : {curBlock.nRequestTime}, reqType :{sReqMsg}, answer : {answer}");
#endif
                    curBlock.fRatio = answer.Item1;
                    curBlock.isTarget = answer.Item2; // 결과 집어넣고

                    accessor.Write(nBlockOffSet + nCurIdxPtr * nStructSize, ref curBlock); // 다시 제자리에 넣기
                    nCurMyPtr = (nCurMyPtr + 1) % nStructStepNum; // 한칸 올리고

                    accessor.Write(nPtrOffSet + SERVER_POINTER_LOC, ref nCurMyPtr); // 내 ptr 집어넣기
                    accessor.Read(nPtrOffSet + USER_POINTER_LOC, out nCurOtherPtr); // 클라이언트 ptr 받아오기

                    

                }
            }
        }

      
        public int nCurAITestNum = 35;

        // 매수 머신러닝 앙상블 예측
        public Tuple<float, bool> CalculateBuyMLResult()
        {
            double[] fTest = new double[curBlock.nFeatureLen];

            unsafe
            {
                for (int i = 0; i < curBlock.nFeatureLen; i++)
                    fTest[i] = curBlock.fArr[i];
            }

            float?[] answerArr = new float?[nCurAITestNum];


            float fSucCrit = 0.65f;
            float fSucCnt = 0;
            for (int i = 0; i < nCurAITestNum; i++)
            {
                answerArr[i] = aIStarter.arrRFCGroup1[i].Score(fTest);
                if (answerArr[i] == 0)
                    fSucCnt++;
            }


            return new Tuple<float,bool>( fSucCnt / nCurAITestNum, (fSucCnt / nCurAITestNum) > fSucCrit);
        }

        public Tuple<float, bool> CalculateFakeMLResult()
        {
            double[] fTest = new double[curBlock.nFeatureLen];

            unsafe
            {
                for (int i = 0; i < curBlock.nFeatureLen; i++)
                    fTest[i] = curBlock.fArr[i];
            }

            float?[] answerArr = new float?[nCurAITestNum];


            float fSucCrit = 0.45f;
            float fSucCnt = 0;
            for (int i = 0; i < nCurAITestNum; i++)
            {
                answerArr[i] = aIStarter.arrRFCGroup1[i].Score(fTest);
                if (answerArr[i] == 0)
                    fSucCnt++;
            }


            return new Tuple<float, bool>(fSucCnt / nCurAITestNum, (fSucCnt / nCurAITestNum) > fSucCrit);
        }

        // 매도 머신러닝 앙상블 예측
        public Tuple<float, bool> CalculateSellMLResult()
        {
            double[] fTest = new double[curBlock.nFeatureLen];

            unsafe
            {
                for (int i = 0; i < curBlock.nFeatureLen; i++)
                    fTest[i] = curBlock.fArr[i];
            }

            float?[] answerArr = new float?[nCurAITestNum];

            float fSellCrit = 0.3f;
            float fSucCnt = 0;

            for (int i = 0; i< nCurAITestNum; i++)
            {
                answerArr[i] = aIStarter.arrRFCGroup1[i].Score(fTest);
                if (answerArr[i] == 0)
                    fSucCnt++;
            }


            return new Tuple<float, bool>(  fSucCnt / nCurAITestNum, (fSucCnt / nCurAITestNum) < fSellCrit);
        }

        public void TestMLResult(int n)
        {
            double test_val = 10;
            var fTest = new double[102] {
                    test_val, test_val, test_val, test_val, test_val, test_val, test_val, test_val, test_val, test_val,
                    test_val, test_val, test_val, test_val, test_val, test_val, test_val, test_val, test_val, test_val,
                    test_val, test_val, test_val, test_val, test_val, test_val, test_val, test_val, test_val, test_val,
                    test_val, test_val, test_val, test_val, test_val, test_val, test_val, test_val, test_val, test_val,
                    test_val, test_val, test_val, test_val, test_val, test_val, test_val, test_val, test_val, test_val,
                    test_val, test_val, test_val, test_val, test_val, test_val, test_val, test_val, test_val, test_val,
                    test_val, test_val, test_val, test_val, test_val, test_val, test_val, test_val, test_val, test_val,
                    test_val, test_val, test_val, test_val, test_val, test_val, test_val, test_val, test_val, test_val,
                    test_val, test_val, test_val, test_val, test_val, test_val, test_val, test_val, test_val, test_val,
                    test_val, test_val, test_val, test_val, test_val, test_val, test_val, test_val, test_val, test_val,
                    test_val, test_val
                };

            float?[] answerArr = new float?[nCurAITestNum];
            
            float fSucCnt = 0;


            for (int i = 0; i < nCurAITestNum; i++)
            {
                answerArr[i] = aIStarter.arrRFCGroup1[i].Score(fTest);
                if (answerArr[i] == 0)
                    fSucCnt++;
            }



            Console.WriteLine($"{n}번째 테스트 완료 : {fSucCnt  / nCurAITestNum}(%)");
        }
    }
}
