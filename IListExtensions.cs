namespace InterfacePerfTest
{
    // Taken from https://stackoverflow.com/questions/273313/randomize-a-listt/1262619#1262619
    internal static class IListExtensions
    {
        private static readonly Random rng = new();

        public static void Shuffle<T>(this IList<T> list)
        {
            int n = list.Count;
            while (n > 1)
            {
                n--;
                int k = rng.Next(n + 1);

                // Swap values
                (list[n], list[k]) = (list[k], list[n]);
            }
        }
    }
}
