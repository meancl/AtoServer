using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Runtime.InteropServices;

namespace MJTradier_AI_Server.Shared_Memory
{
    public unsafe struct SharedAIBlock
    {
        public int nFeatureLen;

        public fixed double fArr[105];

        public double fTarget;
    }
}
