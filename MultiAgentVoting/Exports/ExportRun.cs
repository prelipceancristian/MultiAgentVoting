using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MultiAgentVoting.Exports
{
    internal class ExportRun
    {
        public int Seed {  get; set; }
        public List<ProtocolRunResult> Results { get; set; } = new List<ProtocolRunResult>();
    }
}
