using BlockBuster.Utils;
using System.Collections.Generic;
using System.Linq;

namespace BlockBuster.Score
{

    public class HistoryStack<T>
    {
        private readonly LinkedList<T> items = new();
        private int capacity;

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
                lock (_lockObject)
                {
                    return capacity;
                }
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

        public object _lockObject = new();

        public void Push(T item)
        {
            lock (_lockObject)
            {

                // full
                if (items.Count == Capacity)
                {
                    // we should remove first, because some times, if we exceeded the size of the internal array
                    // the system will allocate new array.
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