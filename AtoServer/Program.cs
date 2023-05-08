using AtoServer.AI;
using AtoServer.Shared_Memory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AtoServer
{
    class Program
    {
        static void Main(string[] args)
        {
            // 이벤트콜 테스트
            Console.WriteLine("이벤트콜 대기");
            EventWaitHandle testEventHandle = new EventWaitHandle(false, EventResetMode.AutoReset, "MySharedMemoryEvent");
            testEventHandle.WaitOne();
            Console.WriteLine("이벤트콜 수신완료");
            MMF mmf = new MMF();

            // AI 테스트
            for (int i = 0; i < 3; i++)
                mmf.TestMLResult(i);

            while (true)
            {
                // Wait for the event to be signaled before attempting to read from the shared memory
                EventWaitHandle eventHandle = new EventWaitHandle(false, EventResetMode.AutoReset, "MySharedMemoryEvent");
                eventHandle.WaitOne();

                mmf.ServeAIService();
            }
        }

    }
}

