using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.IO.MemoryMappedFiles;
using System.Runtime.InteropServices;
using MJTradier_AI_Server.AI;

namespace MJTradier_AI_Server.Shared_Memory
{
    public class MMF
    {
        public const string sMemoryName = "MJTradierMemory";

        public const int SERVER_POINTER_LOC = 0;
        public const int USER_POINTER_LOC = 4;
        public const int nStepPtrSize = 8; 
        public const int nStructStepNum = 1024;

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
                    
                    var answer = CalculateMLResult(); // 머신러닝 계산하기

#if CONSOL
                    Console.WriteLine($"myPtr : {nCurMyPtr}, otherPtr : {nCurOtherPtr}, nCurIdxPtr : {nCurIdxPtr}, blockLen : {curBlock.nFeatureLen} answer : {answer}");
#endif
                    curBlock.fTarget = answer; // 결과 집어넣고

                    accessor.Write(nBlockOffSet + nCurIdxPtr * nStructSize, ref curBlock); // 다시 제자리에 넣기
                    nCurMyPtr = (nCurMyPtr + 1) % nStructStepNum; // 한칸 올리고

                    accessor.Write(nPtrOffSet + SERVER_POINTER_LOC, ref nCurMyPtr); // 내 ptr 집어넣기
                    accessor.Read(nPtrOffSet + USER_POINTER_LOC, out nCurOtherPtr); // 클라이언트 ptr 받아오기
                }
            }
        }

        
        // 머신러닝 앙상블 예측
        public float CalculateMLResult()
        {
            double[] fTest = new double[curBlock.nFeatureLen];

            unsafe
            {
                for (int i = 0; i < curBlock.nFeatureLen; i++)
                    fTest[i] = curBlock.fArr[i];
            }

            float?[] answerArr = new float?[35];

            answerArr[0] = aIStarter.arrRFCGroup1[0].Score(fTest);
            answerArr[1] = aIStarter.arrRFCGroup1[1].Score(fTest);
            answerArr[2] = aIStarter.arrRFCGroup1[2].Score(fTest);
            answerArr[3] = aIStarter.arrRFCGroup1[3].Score(fTest);
            answerArr[4] = aIStarter.arrRFCGroup1[4].Score(fTest);
            answerArr[5] = aIStarter.arrRFCGroup1[5].Score(fTest);
            answerArr[6] = aIStarter.arrRFCGroup1[6].Score(fTest);
            answerArr[7] = aIStarter.arrRFCGroup1[7].Score(fTest);
            answerArr[8] = aIStarter.arrRFCGroup1[8].Score(fTest);
            answerArr[9] = aIStarter.arrRFCGroup1[9].Score(fTest);
            answerArr[10] = aIStarter.arrRFCGroup1[10].Score(fTest);
            answerArr[11] = aIStarter.arrRFCGroup1[11].Score(fTest);
            answerArr[12] = aIStarter.arrRFCGroup1[12].Score(fTest);
            answerArr[13] = aIStarter.arrRFCGroup1[13].Score(fTest);
            answerArr[14] = aIStarter.arrRFCGroup1[14].Score(fTest);
            answerArr[15] = aIStarter.arrRFCGroup1[15].Score(fTest);
            answerArr[16] = aIStarter.arrRFCGroup1[16].Score(fTest);
            answerArr[17] = aIStarter.arrRFCGroup1[17].Score(fTest);
            answerArr[18] = aIStarter.arrRFCGroup1[18].Score(fTest);
            answerArr[19] = aIStarter.arrRFCGroup1[19].Score(fTest);
            answerArr[20] = aIStarter.arrRFCGroup1[20].Score(fTest);
            answerArr[21] = aIStarter.arrRFCGroup1[21].Score(fTest);
            answerArr[22] = aIStarter.arrRFCGroup1[22].Score(fTest);
            answerArr[23] = aIStarter.arrRFCGroup1[23].Score(fTest);
            answerArr[24] = aIStarter.arrRFCGroup1[24].Score(fTest);
            answerArr[25] = aIStarter.arrRFCGroup1[25].Score(fTest);
            answerArr[26] = aIStarter.arrRFCGroup1[26].Score(fTest);
            answerArr[27] = aIStarter.arrRFCGroup1[27].Score(fTest);
            answerArr[28] = aIStarter.arrRFCGroup1[28].Score(fTest);
            answerArr[29] = aIStarter.arrRFCGroup1[29].Score(fTest);
            answerArr[30] = aIStarter.arrRFCGroup1[30].Score(fTest);
            answerArr[31] = aIStarter.arrRFCGroup1[31].Score(fTest);
            answerArr[32] = aIStarter.arrRFCGroup1[32].Score(fTest);
            answerArr[33] = aIStarter.arrRFCGroup1[33].Score(fTest);
            answerArr[34] = aIStarter.arrRFCGroup1[34].Score(fTest);

            
            float fSucCnt = 0;
            for (int i = 0; i< answerArr.Length; i++)
            {
                if (answerArr[i] == 0)
                    fSucCnt++;
            }
#if CONSOL
            Console.WriteLine($"result percent : {(fSucCnt / answerArr.Length)}");
#endif
            return fSucCnt / answerArr.Length;
        }
    }
}
