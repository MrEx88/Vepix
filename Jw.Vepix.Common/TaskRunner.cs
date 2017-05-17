using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Jw.Vepix.Common
{
    public static class TaskRunner
    {
        public async static void WaitAllOneByOne<T, U>(IEnumerable<T> collection, 
            Func<T, Task<U>> theTask, Action<U> theAction, Action onCompletion = null)
        {
            List<Task<U>> tasks = new List<Task<U>>();

            //todo check performance later
            //foreach (var item in collection)
            //{
            //    tasks.Add(theTask(item));
            //}
            Parallel.ForEach(collection, item =>
            {
                tasks.Add(theTask(item));
            });

            while (tasks.Count > 0)
            {
                int index = await Task.Factory.StartNew(() =>
                {
                    return Task.WaitAny(tasks.ToArray());
                });

                theAction(tasks[index].Result);

                tasks.RemoveAt(index);
            }

            onCompletion?.Invoke();
        }
    }
}
