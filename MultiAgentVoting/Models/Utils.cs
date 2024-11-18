namespace MultiAgentVoting.Models
{
    internal static class Utils
    {
        public static Random Rng = new(Seed: 1);

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
