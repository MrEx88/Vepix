using System;
using System.Collections.ObjectModel;

namespace JW.Vepix.Core.Extensions
{
    public static class ObservableCollectionsExtensions
    {
        public static bool RemoveAll<T>(this ObservableCollection<T> collection,
                                        Func<T, bool> condition)
        {
            var itemsRemoved = false;
            for (int i = collection.Count - 1; i >= 0; i--)
            {
                if (condition(collection[i]))
                {
                    collection.RemoveAt(i);
                    itemsRemoved = true;
                }
            }

            return itemsRemoved;
        }
    }
}
