using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utilities.Queues;
using Xunit;

namespace Utilities.Tests
{
    public class QueuesTests
    {
        /// <summary>
        /// Tests if the FixedSizeQueue will dequeue old values to keep its size.
        /// </summary>
        [Fact]
        public void FixedSizeQueueWillDequeuesOldValuesWhenFull()
        {
            int queueSize = 5;

            List<int> dataSet = new List<int> { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 };
            List<int> expected = new List<int> { 5, 6, 7, 8, 9 };

            FixedSizeQueue<int> queue = new FixedSizeQueue<int>(queueSize);

            foreach (int data in dataSet)
            {
                queue.Enqueue(data);
            }

            List<int> actual = queue.ToList<int>();

            Assert.Equal(queueSize, queue.Size);
            Assert.Equal<int>(expected, actual);
        }

        /// <summary>
        /// Checks if the FixedSizeQueue correctly dequeues.
        /// </summary>
        [Fact]
        public void FixedSizeQueueCorrectlyDequeues()
        {
            int expected = 3;

            FixedSizeQueue<int> queue = new FixedSizeQueue<int>(10);

            queue.Enqueue(1);
            queue.Enqueue(2);
            queue.Enqueue(3);
            queue.Enqueue(4);
            queue.Enqueue(5);
            queue.Enqueue(6);

            int overflow;
            queue.TryDequeue(out overflow);
            queue.TryDequeue(out overflow);

            int actual;
            queue.TryDequeue(out actual);

            Assert.Equal(expected, actual);
        }

        /// <summary>
        /// Tests if the IntervalQueue will respect Time Window.
        /// </summary>
        [Fact]
        public void IntervalQueueWillRespectWindowSize()
        {
            TimeSpan windowSize = TimeSpan.FromDays(2.5);
            IntervalQueue<QueueItem> queue = new IntervalQueue<QueueItem>(windowSize, "EventTime");

            // Data Set
            DateTime initialDate = DateTime.UtcNow;
            double timeIncrement = 1;

            List<QueueItem> dataSet = new List<QueueItem>();

            for (int i = 0; i < 10; i++)
            {
                dataSet.Add(new QueueItem()
                {
                    EventTime = initialDate.AddDays((double)i * timeIncrement),
                    Value = i
                });
            }

            List<QueueItem> expected = dataSet.Skip(7).ToList();

            foreach (QueueItem item in dataSet)
            {
                queue.Enqueue(item);
            }

            List<QueueItem> actual = queue.ToList();

            Assert.Equal<QueueItem>(expected, actual);
        }

        /// <summary>
        /// Checks if the IntervalQueue Correctly Dequeues.
        /// </summary>
        [Fact]
        public void IntervalQueueCorrectlyDequeues()
        {
            TimeSpan windowSize = TimeSpan.FromDays(5);
            IntervalQueue<QueueItem> queue = new IntervalQueue<QueueItem>(windowSize, "EventTime");

            DateTime initialDate = DateTime.UtcNow;
            int initialValue = 0;       

            queue.Enqueue(new QueueItem()
            {
                EventTime  = initialDate.AddDays(1),
                Value = initialValue++,
            });

            QueueItem expected = new QueueItem()
            {
                EventTime = initialDate.AddDays(2),
                Value = initialValue++,
            };

            queue.Enqueue(expected);

            queue.Enqueue(new QueueItem()
            {
                EventTime = initialDate.AddDays(3),
                Value = initialValue++,
            });

            QueueItem overflow;
            queue.TryDequeue(out overflow);

            QueueItem actual;
            queue.TryDequeue(out actual);

            Assert.Equal(expected, actual);
        }
    }
}
