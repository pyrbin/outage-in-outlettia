namespace System.Collections.Generic
{
    public static unsafe class QueueExtensions
    {
        public static void AddRange<T>(this Queue<T> queue, IEnumerable<T> enu)
        {
            foreach (T obj in enu) queue.Enqueue(obj);
        }
    }
}
