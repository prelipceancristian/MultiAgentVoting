using MultiAgentVoting.Agents;
using MultiAgentVoting.Models;

namespace MultiAgentVoting.VotingProtocols;

internal class StvProtocol : IVotingProtocol
{
    public Vote Vote(VoterAgent voter, List<Rating> ratings)
    {
        var bestRating = ratings.First().Value;
        var bestCandidates = ratings.Where(r => Math.Abs(r.Value - bestRating) < 1e-6).ToList();
        var candidate = Utils.PickRandom(bestCandidates).Candidate;
        var vote = new Vote(voter, [candidate]);
        return vote;
    }

    public void UpdateVotes(Dictionary<CandidateAgent, List<VoterAgent>> votes, Vote vote)
    {
        if (vote.ViableCandidates.Count != 1)
        {
            throw new Exception("Invalid vote count");
        }
        var candidate = vote.ViableCandidates[0];
        if (!votes.TryGetValue(candidate, out var candidateVoters))
        {
            throw new Exception($"Voter {vote.VoterAgent.Name} failed to vote for {candidate.Name}. The candidate is not registered");
        }
        if (candidateVoters.Contains(vote.VoterAgent))
        {
            throw new Exception($"Voter {vote.VoterAgent.Name} already voted for {candidate.Name}");
        }
        candidateVoters.Add(vote.VoterAgent);
    }

    public ElectionResult EstablishWinner(Dictionary<CandidateAgent, List<VoterAgent>> votes)
    {
        var mostNumberOfVotes = votes.Select(kvp => kvp.Value.Count).Max();
        var totalNumberOfVotes = votes.Select(kvp => kvp.Value.Count).Sum();
        var threshold = totalNumberOfVotes / 2.0;
        if (mostNumberOfVotes > threshold)
        {
            var candidatesWithMostVotes = votes
                .Where(kvp => kvp.Value.Count == mostNumberOfVotes)
                .Select(kvp => kvp.Key)
                .ToList();
            var winner = Utils.PickRandom(candidatesWithMostVotes);
            return new SuccessfulElectionResult(winner);
        }
        
        var leastNumberOfVotes = votes.Select(kvp => kvp.Value.Count).Min();
        var candidatesWithLeastVotes = votes
            .Where(kvp => kvp.Value.Count == leastNumberOfVotes)
            .Select(kvp => kvp.Key)
            .ToList();
        var eliminated = Utils.PickRandom(candidatesWithLeastVotes);
        return new FailedElectionResult(eliminated);
    }
}