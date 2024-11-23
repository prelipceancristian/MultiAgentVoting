using MultiAgentVoting.Agents;
using MultiAgentVoting.Models;

namespace MultiAgentVoting.VotingProtocols;

internal class ApprovalVotingProtocol : IVotingProtocol
{
    public Vote Vote(VoterAgent voter, List<Rating> ratings)
    {
        const double minimalApprovalRating = 0.5;
        var validCandidates = ratings
            .Where(rating => rating.Value >= minimalApprovalRating)
            .Select(rating => rating.Candidate)
            .ToList();
        var vote = new Vote(voter, validCandidates);
        return vote;
    }

    public void UpdateVotes(Dictionary<CandidateAgent, List<VoterAgent>> votes, Vote vote)
    {
        foreach (var candidate in vote.ViableCandidates)
        {
            if (!votes.TryGetValue(candidate, out var candidateVoters))
            {
                throw new Exception($"[Moderator agent] Voter {vote.VoterAgent.Name} failed to vote for {candidate.Name}. The candidate is not registered");
            }
            if (candidateVoters.Contains(vote.VoterAgent))
            {
                throw new Exception($"[Moderator agent] Voter {vote.VoterAgent.Name} already voted for {candidate.Name}");
            }
            candidateVoters.Add(vote.VoterAgent);
        }
    }

    public ElectionResult EstablishWinner(Dictionary<CandidateAgent, List<VoterAgent>> votes)
    {
        var mostNumberOfVotes = votes.Select(kvp => kvp.Value.Count).Max();
        var candidatesWithMostVotes = votes
            .Where(kvp => kvp.Value.Count == mostNumberOfVotes)
            .Select(kvp => kvp.Key)
            .ToList();
        var winner = Utils.PickRandom(candidatesWithMostVotes);
        return new SuccessfulElectionResult(winner);
    }
}