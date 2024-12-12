namespace MultiAgentVoting.Models
{
    internal static class Utils
    {
        public const string ModeratorName = "Moderator";
        
        public static Random Rng = new();

        public static T PickRandom<T>(List<T> list)
        {
            var randomIndex = Rng.Next(list.Count);
            return list[randomIndex];
        }

        public static double GenerateCriteriaEvaluation()
        {
            return Rng.NextDouble() * 10;
        }
    }
}
