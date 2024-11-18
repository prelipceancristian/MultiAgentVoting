using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MultiAgentVoting.Models
{
    internal interface IVotingStrategy
    {
        public IVote DetermineVote(Vote vote);

        public IVote HandleVote(Vote vote);
    }
}
