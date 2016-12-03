namespace System.Collections.Generic
{
    /// <summary>
    /// Extensions to the List class.
    /// </summary>
    public static class ListExtensions
    {
        /// <summary>
        /// Applies a Fisher-Yates in-place shuffle to randomize this list.
        /// </summary>
        public static void Shuffle<T>(this IList<T> list)
        {
            int index = list.Count - 1;
            while (index >= 1) {
                int rand = random.Random.Range(0, index + 1);
                T value = list[rand];
                list[rand] = list[index];
                list[index] = value;
                index--;
            }
        }
    }
}
