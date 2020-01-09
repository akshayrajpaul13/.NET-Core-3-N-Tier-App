using System;
using System.Collections.Generic;
using System.Linq;

namespace Web.Api.Base.Extensions
{
    public static class ListExtensions
    {
        /// <summary>
        /// Indicates if a list is empty.  Passing in null will throw an argument exception.
        /// </summary>
        public static bool Empty<T>(this List<T> list)
        {
            if (list == null) throw new ArgumentNullException("list");

            return !list.Any();
        }

        /// <summary>
        /// Indicates if a list is null or empty
        /// </summary>
        public static bool NullOrEmpty<T>(this List<T> list)
        {
            if (list == null)
                return true;

            return !list.Any();
        }

        public static IEnumerable<TSource> DistinctBy<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector)
        {
            HashSet<TKey> seenKeys = new HashSet<TKey>();
            foreach (TSource element in source)
            {
                if (seenKeys.Add(keySelector(element)))
                {
                    yield return element;
                }
            }
        }

        /// <summary>
        /// Creates a new list containing all the items of the provided list.
        /// </summary>
        public static List<T> Clone<T>(this List<T> list)
        {
            if (list == null)
                return null;

            return list.Select(x => x).ToList();
        }

        /// <summary>
        /// Indicates if this is the last item in the list
        /// </summary>
        public static bool IsLastItem<T>(this List<T> list, T currentItem)
        {
            return list.Count == list.IndexOf(currentItem) + 1;
        }

        /// <summary>
        /// Gets the next item in the list after the current one. Throws an exception if there are
        /// no more items available.
        /// </summary>
        public static T GetNextItem<T>(this List<T> list, T currentItem)
        {
            if (list.IsLastItem(currentItem))
                throw new Exception("Last item in list, can't get next item");

            return list[list.IndexOf(currentItem) + 1];
        }

        /// <summary>
        /// Adds a set of dictionary values to an existing dictionary. An error will occur if a value is duplicated.
        /// </summary>
        public static void AddRange(this Dictionary<string, string> existingValues, Dictionary<string, string> newValues)
        {
            foreach (var newValue in newValues)
            {
                if (existingValues.ContainsKey(newValue.Key))
                    throw new Exception("Dictionary already contains key: " + newValue.Key);

                existingValues.Add(newValue.Key, newValue.Value);
            }
        }

        /// <summary>
        /// Gets the first parameter that isn't null
        /// </summary>
        public static T GetFirstNotNull<T>(params T[] items)
        {
            if (!items.Any())
                throw new Exception("No parameters were provided");

            return items.FirstOrDefault(x => x != null);
        }

        /// <summary>
        /// Gets the first parameter that isn't null, empty or whitespace
        /// </summary>
        public static string GetFirstSet(params string[] items)
        {
            if (!items.Any())
                throw new Exception("No parameters were provided");

            return items.FirstOrDefault(x => x.IsSet());
        }

        /// <summary>
        /// Returns everything in the list, excluding the first item
        /// </summary>
        public static List<T> GetAllButFirst<T>(this List<T> items)
        {
            if (items == null) throw new ArgumentNullException(nameof(items));

            if (!items.Any())
                return items;

            return items.Skip(1).ToList();
        }


        /// <summary>
        /// Checks if two lists contain the same values ignoring order
        /// From: https://stackoverflow.com/questions/3669970/compare-two-listt-objects-for-equality-ignoring-order
        /// </summary>
        public static bool ScrambledEquals<T>(this IEnumerable<T> list1, IEnumerable<T> list2)
        {
            var cnt = new Dictionary<T, int>();
            foreach (T s in list1)
            {
                if (cnt.ContainsKey(s))
                {
                    cnt[s]++;
                }
                else
                {
                    cnt.Add(s, 1);
                }
            }
            foreach (T s in list2)
            {
                if (cnt.ContainsKey(s))
                {
                    cnt[s]--;
                }
                else
                {
                    return false;
                }
            }
            return cnt.Values.All(c => c == 0);
        }
    }
}
