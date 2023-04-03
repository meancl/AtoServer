using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MJTradier_AI_Server.AI
{
    // (cur - D0) / (D1 - D2)
    public class ScaleDatas
    {
        public DateTime dTime { get; set; }
        public string sScaleMethod { get; set; }
        public string sVariableName { get; set; }
        public string sModelName { get; set; }
        public double fD0 { get; set; }
        public double fD1 { get; set; }
        public double fD2 { get; set; }
        public int nSeq { get; set; }
    }
}
