using ActressMas;
using MultiAgentVoting.Models;

namespace MultiAgentVoting.Agents
{
    internal class VoterAgent : Agent
    {
        private Policy Policy { get; }
        private List<Rating> CandidatesRatings { get; set; } = [];

        public VoterAgent(string name, Policy policy)
        {
            Name = name;
            Policy = policy;
        }

        public override void Act(Message message)
        {
            var messageContent = (MessageContent)message.ContentObj;

            switch (messageContent.Action)
            {
                case MessageAction.Vote:
                    HandleVote();
                    break;
                case MessageAction.Winner:
                    Stop();
                    break;
                case MessageAction.Register:
                case MessageAction.VoteResponse:
                default:
                    throw new Exception("Could not parse message action");
            }
        }

        private void HandleVote()
        {
            CandidatesRatings = RateVoters();
            var votingProtocol = SharedKnowledgeService.VotingProtocol;
            var vote = votingProtocol.Vote(this, CandidatesRatings);

            var responseMessageContent = new MessageContent(MessageAction.VoteResponse, vote);
            Send(Utils.ModeratorName, responseMessageContent);
        }

        private List<Rating> RateVoters()
        {
            var candidates = SharedKnowledgeService.Registrations;
            var candidatesRatings = candidates
                .Select(RateCandidate)
                .OrderByDescending(rating => rating.Value)
                .ToList();

            return candidatesRatings;
        }

        // public MultipleVote GetApprovalVote()
        // {
        //     const double minimalApprovalRating = 0.5;
        //     var validCandidates = CandidatesRatings
        //         .Where(rating => rating.Value >= minimalApprovalRating)
        //         .Select(rating => rating.Candidate)
        //         .ToList();
        //     var vote = new MultipleVote(this, validCandidates);
        //     return vote;
        // }

        private Rating RateCandidate(CandidateAgent candidate)
        {
            var candidatePolicy = candidate.Policy;
            if (candidatePolicy.CriteriaEvaluations.Count != Policy.CriteriaEvaluations.Count)
            {
                throw new Exception("Somehow the candidate and the voter do not have the same number of criteria");
            }

            var sum = 0.0;
            foreach (var (criteriaId, voterEvaluation) in Policy.CriteriaEvaluations)
            {
                var candidateEvaluation = candidatePolicy.CriteriaEvaluations[criteriaId];
                sum += 1 / (Math.Abs(candidateEvaluation - voterEvaluation) + 1);
            }

            var ratingValue = sum / Policy.CriteriaEvaluations.Count;

            var voterRating = new Rating(candidate, ratingValue);
            return voterRating;
        }
    }
}