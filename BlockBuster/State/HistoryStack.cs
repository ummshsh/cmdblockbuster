using BlockBuster.Settings;
using BlockBuster.Utils;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace BlockBuster.Score
{
    public class HistoryStack<T>
    {
        private readonly LinkedList<T> items = new();
        private int capacity;

        public object _lockObject = new();

        public List<T> Items
        {
            get
            {
                lock (_lockObject)
                {
                    return items.ToList();
                }
            }
        }

        public int Capacity
        {
            get
            {
                return capacity;
            }
            private set
            {
                capacity = value;
            }
        }

        public HistoryStack(int capacity)
        {
            Capacity = capacity;
        }

        public void Push(T item)
        {
            lock (_lockObject)
            {
                ////Debug.WriteLineIf(Config.EnableDebugOutput, "HistoryStack push:" + item);
                if (items.Count == Capacity)
                {
                    items.RemoveFirst();
                    items.AddLast(item);
                }
                else
                {
                    items.AddLast(new LinkedListNode<T>(item));
                }
            }
        }

        public T Pop()
        {
            lock (_lockObject)
            {
                if (items.Count == 0)
                {
                    return default;
                }
                var ls = items.Last;
                items.RemoveLast();
                return ls == null ? default : ls.Value;
            }
        }

        /// <param name="index">From the top of the stack to bottom index. 0 peeks at last element</param>
        /// <returns></returns>
        public T Peek(int index)
        {
            lock (_lockObject)
            {
                if (items.Count > index)
                {
                    return Items.ReverseList().ToList().ElementAt(index);
                }

                return default;
            }
        }
    }
}