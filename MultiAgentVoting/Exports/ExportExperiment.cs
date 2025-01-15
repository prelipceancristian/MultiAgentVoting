using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MultiAgentVoting.Exports
{
    internal class ExportExperiment
    {
        public int CandidateCount { get; set; }
        public int VoterCount { get; set; }
        public List<ExportRun> ExportRuns { get; set; } = new List<ExportRun>();

    }
}
