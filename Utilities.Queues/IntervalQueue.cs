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
        /// Window Size.
        /// </summary>
        public TimeSpan Interval { get; private set; }

        /// <summary>
        /// Name of the Time Property.
        /// </summary>
        public string TimePropertyName { get; private set; }

        public IntervalQueue(TimeSpan interval)
        {
            Interval = interval;
        }

        public IntervalQueue(TimeSpan interval, string timePropertyName)
        {
            Interval = interval;
            TimePropertyName = timePropertyName;

        }

        // TODO: Implementing method to ensure Type has Time Property.
        private void EnsureTypeHasProperty()
        {
            if (string.IsNullOrEmpty(TimePropertyName))
            {
                //if (TypeHasTimeProperty("Timestamp")) 
            }
            else
            {

            }
            //throw new MissingFieldException(type.Name, TimePropertyName);
        }

        /// <summary>
        /// Checks if type T has a property called 'TimePropertyName' of type 'DateTime'.
        /// </summary>
        /// <param name="timePropertyName">Property Name</param>
        /// <returns>If property exists and is a DateTime.</returns>
        private bool TypeHasTimeProperty(string timePropertyName)
        {
            Type type = typeof(T);
            PropertyInfo property = type.GetProperty(timePropertyName);
            if (property == null)
            {
                return false;
            }

            return property.PropertyType == typeof(DateTime);
        }
        public new void Enqueue(T item)
        {

        }
    }
}
