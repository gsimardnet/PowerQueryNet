using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace PowerQueryNet.Client
{
    /// <summary>
    /// Collection of instances of parameter in the Power Query (M) formulas.
    /// </summary>
    public class Parameters : IEnumerable, ICollection
    {
        private List<Parameter> parameterList;

        /// <summary>
        /// Initializes a new instance of the Parameters class.
        /// </summary>
        public Parameters()
        {
            parameterList = new List<Parameter>();
        }

        /// <summary>
        /// Adds a Parameter to the collection.
        /// </summary>
        /// <param name="parameter"></param>
        public void Add(object parameter)
        {
            if (parameter is Parameter)
            {
                parameterList.Add((Parameter)parameter);
            }
        }

        /// <summary>
        /// Removes all Parameter from the collection.
        /// </summary>
        public void Clear()
        {
            parameterList = new List<Parameter>();
        }

        /// <summary>
        /// Removes the first occurence of a Parameter from the collection.
        /// </summary>
        /// <param name="parameter"></param>
        public void Remove(Parameter parameter)
        {
            parameterList.Remove(parameter);
        }

        /// <summary>
        /// Gets a Parameter by index.
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public Parameter this[int index]
        {
            get { return parameterList[index]; }
            set { parameterList.Insert(index, value); }
        }

        /// <summary>
        /// Copies the elements of Parameters to a new array.
        /// </summary>
        /// <returns></returns>
        public Parameter[] ToArray()
        {
            return parameterList.ToArray();
        }

        /// <summary>
        /// Returns an enumerator that iterates through the Parameters collection.
        /// </summary>
        /// <returns></returns>
        public IEnumerator GetEnumerator()
        {
            return parameterList.GetEnumerator();
        }

        /// <summary>
        /// Gets the number of Parameter in the collection.
        /// </summary>
        public int Count
        {
            get
            {
                return parameterList.Count;
            }
        }

        /// <summary>
        /// Gets an object that can be used to synchronize access to the ICollection.
        /// </summary>
        public object SyncRoot
        {
            get
            {
                return this;
            }
        }

        /// <summary>
        /// Gets a value indicating whether access to the ICollection is synchronized (thread safe).
        /// </summary>
        public bool IsSynchronized
        {
            get
            {
                return false;
            }
        }

        /// <summary>
        /// NotImplemented yet.
        /// </summary>
        /// <param name="array">The one-dimensional array that is the destination of the elements copied from the current array.</param>
        /// <param name="index">A 32-bit integer that represents the index in array at which copying begins.</param>
        public void CopyTo(Array array, int index)
        {
            throw new NotImplementedException();
        }
    }
}
