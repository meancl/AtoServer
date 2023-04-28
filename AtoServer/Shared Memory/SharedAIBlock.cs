using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Runtime.InteropServices;

namespace AtoServer.Shared_Memory
{
    public unsafe struct SharedAIBlock
    {
        public fixed char cCodeArr[6];

        public int nRequestType;

        public int nRequestTime;

        public int nFeatureLen;

        public fixed double fArr[105];

        public double fRatio;

        public bool isTarget;
    }
}
