using ActressMas;
using MultiAgentVoting.Models;

namespace MultiAgentVoting.Agents;

internal class VoterAgent : Agent
{
    private Policy Policy { get; }
    private List<Rating> CandidatesRatings { get; set; } = [];

    public VoterAgent(string name, Policy policy)
    {
        Name = name;
        Policy = policy;
    }

    public override void Setup()
    {
        SharedKnowledgeService.RegisterVoter(this);
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

            case MessageAction.VoteResponse:

            case MessageAction.Start:

            default:
                throw new Exception("Message action unsupported");
        }
    }

    private void HandleVote()
    {
        CandidatesRatings = RateCandidates();
        var votingProtocol = SharedKnowledgeService.VotingProtocol;
        var vote = votingProtocol.Vote(this, CandidatesRatings);

        var responseMessageContent = new MessageContent(MessageAction.VoteResponse, vote);
        Send("Moderator", responseMessageContent);
    }

    private List<Rating> RateCandidates()
    {
        var candidates = SharedKnowledgeService.Candidates.Select(kvp => kvp.Key);
        var candidatesRatings = candidates
            .Select(RateCandidate)
            .OrderByDescending(rating => rating.Value)
            .ToList();

        return candidatesRatings;
    }

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