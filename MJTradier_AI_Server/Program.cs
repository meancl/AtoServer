using MJTradier_AI_Server.AI;
using MJTradier_AI_Server.Shared_Memory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MJTradier_AI_Server
{
    class Program
    {
        static void Main(string[] args)
        {
            MMF mmf = new MMF();
            mmf.TestMLResult();

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

