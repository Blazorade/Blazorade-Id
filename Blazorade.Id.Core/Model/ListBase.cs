using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blazorade.Id.Model
{
    /// <summary>
    /// A base implementation of the <see cref="IList{T}"/> interface.
    /// </summary>
    /// <typeparam name="T">The type of items to contain in the list.</typeparam>
    /// <remarks>
    /// This class provides capabilities to work with the items stored in the list that
    /// are not possible by inheriting directly from <see cref="List{T}"/>.
    /// </remarks>
    public class ListBase<T> : IList<T>
    {
        /// <summary>
        /// Creates a new instance of the class.
        /// </summary>
        protected ListBase() { }

        /// <summary>
        /// Creates a new instance of the class initializing it with the given collection.
        /// </summary>
        protected ListBase(IEnumerable<T> collection)
        {
            if (collection is null) throw new ArgumentNullException(nameof(collection));
            this.InnerList.AddRange(collection);
        }

        private List<T> InnerList = new List<T>();

        /// <inheritdoc/>
        public T this[int index] { get => ((IList<T>)InnerList)[index]; set => ((IList<T>)InnerList)[index] = value; }

        /// <inheritdoc/>
        public int Count => ((ICollection<T>)InnerList).Count;

        /// <inheritdoc/>
        public bool IsReadOnly => ((ICollection<T>)InnerList).IsReadOnly;

        /// <inheritdoc/>
        public void Add(T item)
        {
            ((ICollection<T>)InnerList).Add(item);
        }

        /// <inheritdoc/>
        public void Clear()
        {
            ((ICollection<T>)InnerList).Clear();
        }

        /// <inheritdoc/>
        public bool Contains(T item)
        {
            return ((ICollection<T>)InnerList).Contains(item);
        }

        /// <inheritdoc/>
        public void CopyTo(T[] array, int arrayIndex)
        {
            ((ICollection<T>)InnerList).CopyTo(array, arrayIndex);
        }

        /// <inheritdoc/>
        public IEnumerator<T> GetEnumerator()
        {
            return ((IEnumerable<T>)InnerList).GetEnumerator();
        }

        /// <inheritdoc/>
        public int IndexOf(T item)
        {
            return ((IList<T>)InnerList).IndexOf(item);
        }

        /// <inheritdoc/>
        public void Insert(int index, T item)
        {
            ((IList<T>)InnerList).Insert(index, item);
        }

        /// <inheritdoc/>
        public bool Remove(T item)
        {
            return ((ICollection<T>)InnerList).Remove(item);
        }

        /// <inheritdoc/>
        public void RemoveAt(int index)
        {
            ((IList<T>)InnerList).RemoveAt(index);
        }

        /// <inheritdoc/>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable)InnerList).GetEnumerator();
        }
    }
}
