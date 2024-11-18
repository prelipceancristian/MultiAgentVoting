using ActressMas;
using MultiAgentVoting.Models;

namespace MultiAgentVoting.Agents
{
    internal class VoterAgent : Agent
    {
        public Policy Policy { get; set; }
        public List<Rating> CandidatesRatings { get; set; } = [];

        public VoterAgent(string name, Policy policy)
        { 
            Name = name;
            Policy = policy;
        }

        public override void Act(Message message)
        {
            var messageContent = (MessageContent)message.ContentObj;
            if (messageContent is null)
            {
                throw new Exception("Could not parse message content");
            }

            switch(messageContent.Action)
            {
                case MessageAction.Vote:
                    HandleVote(messageContent); 
                    break;
                default:
                    throw new Exception("Could not parse message action");
            }
        }

        public void HandleVote(MessageContent messageContent)
        {
            CandidatesRatings = RateVoters();
            var votingProtocol = SharedKnowledgeService.VotingProtocol;
            if (votingProtocol == VotingProtocol.Plurality)
            {
                var vote = GetPluralityVote();
                Send("Monitor", vote);
            }
            else if (votingProtocol == VotingProtocol.Approval)
            {
                var vote = GetApprovalVote();
                Send("Monitor", vote);
            }
            else if (votingProtocol == VotingProtocol.SingleTransferable)
            {
                var vote = GetPluralityVote();
                Send("Monitor", vote);
            }
            else 
            {
                throw new Exception("Invalid voting protocol");
            }
        }

        public List<Rating> RateVoters()
        {
            var candidates = SharedKnowledgeService.Registrations;
            var candidatesRatings = candidates
                .Select(RateCandidate)
                .OrderByDescending(rating => rating.Value)
                .ToList();

            return candidatesRatings;
        }

        public SingularVote GetPluralityVote()
        {
            return GetSingularVote();
        }

        public SingularVote GetSingleTransferableVote()
        {
            return GetSingularVote();
        }

        private SingularVote GetSingularVote()
        {
            var candidate = CandidatesRatings.First().Candidate;
            var vote = new SingularVote(this, candidate);
            return vote;
        }

        public MultipleVote GetApprovalVote()
        {
            const double minimalApprovalRating = 0.5;
            var validCandidates = CandidatesRatings
                .Where(rating => rating.Value >= minimalApprovalRating)
                .Select(rating => rating.Candidate)
                .ToList();
            var vote = new MultipleVote(this, validCandidates);
            return vote;
        }

        public Rating RateCandidate(CandidateAgent candidate)
        {
            var candidatePolicy = candidate.Policy;
            if (candidatePolicy.CriteriaEvaluations.Count != Policy.CriteriaEvaluations.Count)
            {
                throw new Exception("Somehow the candidate and the voter do not have the same number of criterias");
            }

            var sum = 0.0;
            foreach (var kvp in Policy.CriteriaEvaluations)
            {
                var candidateCriteriaEvaluation = candidatePolicy.CriteriaEvaluations[kvp.Key];
                var voterCriteriaEvaluation = kvp.Value;
                sum += 1 / (Math.Abs(candidateCriteriaEvaluation - voterCriteriaEvaluation) + 1);
            }
            var ratingValue = sum / CandidatesRatings.Count;

            var voterRating = new Rating(candidate, ratingValue);
            return voterRating;
        }
    }
}
