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

        public const int nStepPtrSize = 8; // 4 * 2
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
        // 필용한점
        // 순환 큐 기능(check)
        // 모니터링 기능 (instead of 무한루프)

        public MMF()
        {

          
            nTotalMemorySize = nStepPtrSize + nStructStepNum * sizeof(int) + nStructSize * nStructStepNum;

            nPtrOffSet = 0;
            nQueueOffSet = nPtrOffSet + nStepPtrSize;
            nBlockOffSet = nQueueOffSet + nStructStepNum * sizeof(int);

            aIStarter = new AIStarter();

            CreateSharedMemory();
            HandleServerMemory();
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
                // 데이터 쓰기
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

        public void HandleServerMemory()
        {
            /*
             * 1. 데이터를 읽어 nPtr1 과 nPtr2의 차를 구한다
             * 2. 처리해야할 번째 데이터들을 한번에 가져온다
             * 3. 데이터를 계산한다
             * 4. nPtr2 카운트를 한칸 늘리고 남은 데이터가 있다면 3번부터 다시 반복한다
             */
            using (var accessor = mappedMemory.CreateViewAccessor())
            {
                accessor.Read(nPtrOffSet + 0, out nCurMyPtr);
                accessor.Read(nPtrOffSet + 4, out nCurOtherPtr);
                
                while (CheckIsDataComed()) // 데이타 있다면 ??
                {
                    accessor.Read(nBlockOffSet + nCurMyPtr * nStructSize, out curBlock);

                    var answer = CalculateMLResult();
                    curBlock.fTarget = answer; // 답을 찾고 넣는다.

                    accessor.Write(nBlockOffSet + nCurMyPtr * nStructSize, ref curBlock);
                    nCurMyPtr = (nCurMyPtr + 1) % nStructStepNum; // ??

                    accessor.Write(nPtrOffSet + 0, ref nCurMyPtr);
                    accessor.Read(nPtrOffSet + 4, out nCurOtherPtr);
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

            float? answer = aIStarter.arrRFCGroup1[0].Score(fTest);

            return (float)answer;
        }
    }
}
