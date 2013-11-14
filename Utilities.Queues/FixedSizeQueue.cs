/*
 * I saw this solution on StackOverflow. It helped me a lot of times.
 */

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utilities.Queues
{
    public class FixedSizeQueue<T> : ConcurrentQueue<T>
    {
        /// <summary>
        /// Size of the queue.
        /// </summary>
        public int Size { get; private set; }

        public FixedSizeQueue(int size)
        {
            Size = size;
        }

        /// <summary>
        /// Adds a new item in the Queue. Old values will be dequeued.
        /// </summary>
        /// <param name="item">Item do be queued.</param>
        public new void Enqueue(T item)
        {
            base.Enqueue(item);
            lock(this)
            {
                while (base.Count > Size)
                {
                    T overflow;
                    base.TryDequeue(out overflow);
                }
            }
        }
    }
}
