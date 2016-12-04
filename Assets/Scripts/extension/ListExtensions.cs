namespace System.Collections.Generic
{
using System.Linq;

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

        /// <summary>
        /// From <http://stackoverflow.com/questions/3669970/compare-two-listt-objects-for-equality-ignoring-order>.
        /// Compare the contents of two lists, regardless of order.
        /// </summary>
        public static bool ScrambledEquals<T>(this IList<T> list1, IList<T> list2) {
          var cnt = new Dictionary<T, int>();
          foreach (T s in list1) {
            if (cnt.ContainsKey(s)) {
              cnt[s]++;
            } else {
              cnt.Add(s, 1);
            }
          }
          foreach (T s in list2) {
            if (cnt.ContainsKey(s)) {
              cnt[s]--;
            } else {
              return false;
            }
          }
          return cnt.Values.All(c => c == 0);
        }
    }
}
