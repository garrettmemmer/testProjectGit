using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace AnyPath.Managed.Finders.Common
{
    /// <summary>
    /// Managed priority queue
    /// </summary>
    public class MinHeap<T> : IEnumerable<T>
    {
        private List<T> values;
        private IComparer<T> comparer;

        public MinHeap(IEnumerable<T> items, IComparer<T> comparer)
        {
            this.comparer = comparer;
            values = new List<T>();
            values.Add(default(T));
            values.AddRange(items);

            for (int i = values.Count / 2; i >= 1; i--)
            {
                BubbleDown(i);
            }
        }

        public void Clear()
        {
            values.Clear();
            values.Add(default(T));
        }

        public MinHeap(IEnumerable<T> items) : this(items, Comparer<T>.Default) { }

        public MinHeap(IComparer<T> comparer) : this(new T[0], comparer) { }

        public MinHeap() : this(Comparer<T>.Default) { }

        public int Count => values.Count - 1;

        public T Min => values[1];

        /// <summary>
        /// Extract the smallest element.
        /// </summary>
        /// <exception cref="InvalidOperationException"></exception>
        public T ExtractMin()
        {
            int count = Count;

            #if ENABLE_UNITY_COLLECTIONS_CHECKS
            if (count == 0)
            {
                throw new InvalidOperationException("Heap is empty.");
            }
            #endif

            var min = Min;
            values[1] = values[count];
            values.RemoveAt(count);

            if (values.Count > 1)
            {
                BubbleDown(1);
            }

            return min;
        }


        public void Push(T item)
        {
            values.Add(item);
            BubbleUp(Count);
        }

        private void BubbleUp(int index)
        {
            int parent = index / 2;
            while (index > 1 && CompareResult(parent, index) > 0)
            {
                Exchange(index, parent);
                index = parent;
                parent /= 2;
            }
        }

        private void BubbleDown(int index)
        {
            int min;

            while (true)
            {
                int left = index * 2;
                int right = index * 2 + 1;

                if (left < values.Count &&
                    CompareResult(left, index) < 0)
                {
                    min = left;
                }
                else
                {
                    min = index;
                }

                if (right < values.Count &&
                    CompareResult(right, min) < 0)
                {
                    min = right;
                }

                if (min != index)
                {
                    Exchange(index, min);
                    index = min;
                }
                else
                {
                    return;
                }
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private int CompareResult(int index1, int index2)
        {
            return comparer.Compare(values[index1], values[index2]);
        }

        private void Exchange(int index, int max)
        {
            var tmp = values[index];
            values[index] = values[max];
            values[max] = tmp;
        }

        public IEnumerator<T> GetEnumerator() => values.Skip(1).GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}