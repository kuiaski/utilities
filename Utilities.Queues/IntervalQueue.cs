using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Utilities.Queues
{
    public class IntervalQueue<T> : ConcurrentQueue<T>
    {
        /// <summary>
        /// Type of the argument.
        /// </summary>
        private Type typeOfT;

        /// <summary>
        /// If the TimePropertyName is specified, its PropertyInfo is stored for future use. 
        /// </summary>
        private PropertyInfo timeProperty;

        /// <summary>
        /// Window Size.
        /// </summary>
        public TimeSpan Interval { get; private set; }

        /// <summary>
        /// Name of the Time Property.
        /// </summary>
        public string TimePropertyName { get; private set; }

        /// <summary>
        /// Whether 
        /// </summary>
        private bool UseSystemTime;

        /// <summary>
        /// DateTime of the first element in Queue.
        /// </summary>
        public DateTime FirstDate;

        /// <summary>
        /// DateTime of the last element in Queue.
        /// </summary>
        public DateTime LastDate;

        /// <summary>
        /// Queue for event Dates. 
        /// </summary>
        private ConcurrentQueue<DateTime> DatesQueue;
        
        /// <summary>
        /// Creates a new instance of a IntervalQueue using enqueueing Time as Date handler.
        /// </summary>
        /// <param name="interval">Window Size.</param>
        public IntervalQueue(TimeSpan interval)
        {
            OnConstruct(interval, "");
        }

        /// <summary>
        /// Creates a new instance of a IntervalQueue, retrieving Time from an object's property.
        /// </summary>
        /// <param name="interval"></param>
        /// <param name="timePropertyName"></param>
        public IntervalQueue(TimeSpan interval, string timePropertyName)
        {
            OnConstruct(interval, timePropertyName);
        }

        /// <summary>
        /// Generic function to be called in every constructor to avoid code redundancy.
        /// </summary>
        /// <param name="interval">Window Size in TimeSpan</param>
        /// <param name="timePropertyName">Name of the Time Property</param>
        private void OnConstruct(TimeSpan interval, string timePropertyName)
        {
            Interval = interval;
            TimePropertyName = timePropertyName;
            typeOfT = typeof(T);
            UseSystemTime = string.IsNullOrEmpty(timePropertyName);
            EnsureTypeHasDateTimeProperty();
            DatesQueue = new ConcurrentQueue<DateTime>();
        }

        /// <summary>
        /// Ensures T has the property defined in TimePropertyName of type DateTime.
        /// TimePropertyName is case sensitive.
        /// </summary>
        private void EnsureTypeHasDateTimeProperty()
        {
            if (!UseSystemTime)
            {
                PropertyInfo property = typeOfT.GetProperty(TimePropertyName);
                if (property == null) throw new MissingFieldException(typeOfT.FullName, TimePropertyName);
                if (property.PropertyType != typeof(DateTime)) throw new MissingFieldException(string.Format("Property '{0}.{1}' is not of DateTime type.", typeOfT.FullName, TimePropertyName));
                timeProperty = property;
            }
        }

        /// <summary>
        /// Adds a new item to the queue. 
        /// Old itens will be dequeued until all elements in queue are within the Time Window.
        /// </summary>
        /// <param name="item">Item to be enqueued.</param>
        public new void Enqueue(T item)
        {
            DateTime currentTime = UseSystemTime ? DateTime.UtcNow : (DateTime)timeProperty.GetValue(item);

            base.Enqueue(item);
            DatesQueue.Enqueue(currentTime);

            lock (this)
            {
                // First Date
                DatesQueue.TryPeek(out FirstDate);

                // Last Date
                LastDate = currentTime;

                while (LastDate.Subtract(FirstDate).CompareTo(Interval) > 0)
                {
                    T overflow;
                    base.TryDequeue(out overflow);

                    DateTime dateOverflow;
                    DatesQueue.TryDequeue(out dateOverflow);

                    // First Date
                    DatesQueue.TryPeek(out FirstDate);
                }
            }
        }

        /// <summary>
        /// Tries to remove an item from the Queue.
        /// </summary>
        /// <param name="item">Item to be removed from the Queue.</param>
        /// <returns>TRUE if succeeded, FALSE if not.</returns>
        public new bool TryDequeue(out T item)
        {
            DateTime dateOverflow;
            return base.TryDequeue(out item) && DatesQueue.TryDequeue(out dateOverflow);
        }
    }
}
