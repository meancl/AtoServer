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

