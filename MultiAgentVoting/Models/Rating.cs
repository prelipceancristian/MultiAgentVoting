using MultiAgentVoting.Agents;

namespace MultiAgentVoting.Models
{
    //TODO: this could be a record
    internal class Rating
    {
        public CandidateAgent Candidate { get; set; }

        public double Value { get; set; }

        public Rating(CandidateAgent candidate, double rating)
        {
            Candidate = candidate;
            Value = rating;
        }
    }
}
