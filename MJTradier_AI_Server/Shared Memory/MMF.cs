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

        public void DisposeSharedMemory()
        {
            mappedMemory.Dispose();
        }

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
                accessor.Read(nPtrOffSet + SERVER_POINTER_LOC, out nCurMyPtr);
                accessor.Read(nPtrOffSet + USER_POINTER_LOC, out nCurOtherPtr);
                
                while (CheckIsDataComed()) 
                {
                    accessor.Read(nBlockOffSet + nCurMyPtr * nStructSize, out curBlock);

                    var answer = CalculateMLResult();
                    curBlock.fTarget = answer;

                    accessor.Write(nBlockOffSet + nCurMyPtr * nStructSize, ref curBlock);
                    nCurMyPtr = (nCurMyPtr + 1) % nStructStepNum;

                    accessor.Write(nPtrOffSet + SERVER_POINTER_LOC, ref nCurMyPtr);
                    accessor.Read(nPtrOffSet + USER_POINTER_LOC, out nCurOtherPtr);
                }
            }
        }

        

        public float CalculateMLResult()
        {
            double[] fTest = new double[curBlock.nFeatureLen];

            unsafe
            {
                for (int i = 0; i < curBlock.nFeatureLen; i++)
                    fTest[i] = curBlock.fArr[i];
            }

            float? answer0 = aIStarter.arrRFCGroup1[0].Score(fTest);
            float? answer1 = aIStarter.arrRFCGroup1[1].Score(fTest);
            float? answer2 = aIStarter.arrRFCGroup1[2].Score(fTest);
            float? answer3 = aIStarter.arrRFCGroup1[3].Score(fTest);
            float? answer4 = aIStarter.arrRFCGroup1[4].Score(fTest);
            float? answer5 = aIStarter.arrRFCGroup1[5].Score(fTest);
            float? answer6 = aIStarter.arrRFCGroup1[6].Score(fTest);
            float? answer7 = aIStarter.arrRFCGroup1[7].Score(fTest);
            float? answer8 = aIStarter.arrRFCGroup1[8].Score(fTest);
            float? answer9 = aIStarter.arrRFCGroup1[9].Score(fTest);
            float? answer10 = aIStarter.arrRFCGroup1[10].Score(fTest);
            float? answer11 = aIStarter.arrRFCGroup1[11].Score(fTest);
            float? answer12 = aIStarter.arrRFCGroup1[12].Score(fTest);
            float? answer13 = aIStarter.arrRFCGroup1[13].Score(fTest);
            float? answer14 = aIStarter.arrRFCGroup1[14].Score(fTest);
            float? answer15 = aIStarter.arrRFCGroup1[15].Score(fTest);
            float? answer16 = aIStarter.arrRFCGroup1[16].Score(fTest);
            float? answer17 = aIStarter.arrRFCGroup1[17].Score(fTest);
            float? answer18 = aIStarter.arrRFCGroup1[18].Score(fTest);
            float? answer19 = aIStarter.arrRFCGroup1[19].Score(fTest);
            float? answer20 = aIStarter.arrRFCGroup1[20].Score(fTest);
            float? answer21 = aIStarter.arrRFCGroup1[21].Score(fTest);
            float? answer22 = aIStarter.arrRFCGroup1[22].Score(fTest);
            float? answer23 = aIStarter.arrRFCGroup1[23].Score(fTest);
            float? answer24 = aIStarter.arrRFCGroup1[24].Score(fTest);
            float? answer25 = aIStarter.arrRFCGroup1[25].Score(fTest);
            float? answer26 = aIStarter.arrRFCGroup1[26].Score(fTest);
            float? answer27 = aIStarter.arrRFCGroup1[27].Score(fTest);
            float? answer28 = aIStarter.arrRFCGroup1[28].Score(fTest);
            float? answer29 = aIStarter.arrRFCGroup1[29].Score(fTest);
            float? answer30 = aIStarter.arrRFCGroup1[30].Score(fTest);
            float? answer31 = aIStarter.arrRFCGroup1[31].Score(fTest);
            float? answer32 = aIStarter.arrRFCGroup1[32].Score(fTest);
            float? answer33 = aIStarter.arrRFCGroup1[33].Score(fTest);
            float? answer34 = aIStarter.arrRFCGroup1[34].Score(fTest);

            return (float)answer34;
        }
    }
}
