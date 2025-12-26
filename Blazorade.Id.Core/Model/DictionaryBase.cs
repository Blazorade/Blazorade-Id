using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blazorade.Id.Model
{
    /// <summary>
    /// A base implementation of <see cref="IDictionary{TKey, TValue}"/>.
    /// </summary>
    /// <typeparam name="TKey">The type for the key in the dictionary.</typeparam>
    /// <typeparam name="TValue">The type for the value in the dictionary.</typeparam>
    /// <remarks>
    /// This class provides capabilities to work with the items stored in the dictionary that
    /// are not possible by inheriting directly from <see cref="Dictionary{TKey, TValue}"/>.
    /// </remarks>
    public abstract class DictionaryBase<TKey, TValue> : IDictionary<TKey, TValue> where TKey : notnull
    {
        private Dictionary<TKey, TValue> InnerDictionary = new Dictionary<TKey, TValue>();

        /// <inheritdoc/>
        public TValue this[TKey key] { get => ((IDictionary<TKey, TValue>)InnerDictionary)[key]; set => ((IDictionary<TKey, TValue>)InnerDictionary)[key] = value; }

        /// <inheritdoc/>
        public ICollection<TKey> Keys => ((IDictionary<TKey, TValue>)InnerDictionary).Keys;

        /// <inheritdoc/>
        public ICollection<TValue> Values => ((IDictionary<TKey, TValue>)InnerDictionary).Values;

        /// <inheritdoc/>
        public int Count => ((ICollection<KeyValuePair<TKey, TValue>>)InnerDictionary).Count;

        /// <inheritdoc/>
        public bool IsReadOnly => ((ICollection<KeyValuePair<TKey, TValue>>)InnerDictionary).IsReadOnly;

        /// <inheritdoc/>
        public void Add(TKey key, TValue value)
        {
            ((IDictionary<TKey, TValue>)InnerDictionary).Add(key, value);
        }

        /// <inheritdoc/>
        public void Add(KeyValuePair<TKey, TValue> item)
        {
            ((ICollection<KeyValuePair<TKey, TValue>>)InnerDictionary).Add(item);
        }

        /// <inheritdoc/>
        public void Clear()
        {
            ((ICollection<KeyValuePair<TKey, TValue>>)InnerDictionary).Clear();
        }

        /// <inheritdoc/>
        public bool Contains(KeyValuePair<TKey, TValue> item)
        {
            return ((ICollection<KeyValuePair<TKey, TValue>>)InnerDictionary).Contains(item);
        }

        /// <inheritdoc/>
        public bool ContainsKey(TKey key)
        {
            return ((IDictionary<TKey, TValue>)InnerDictionary).ContainsKey(key);
        }

        /// <inheritdoc/>
        public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
        {
            ((ICollection<KeyValuePair<TKey, TValue>>)InnerDictionary).CopyTo(array, arrayIndex);
        }

        /// <inheritdoc/>
        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
        {
            return ((IEnumerable<KeyValuePair<TKey, TValue>>)InnerDictionary).GetEnumerator();
        }

        /// <inheritdoc/>
        public bool Remove(TKey key)
        {
            return ((IDictionary<TKey, TValue>)InnerDictionary).Remove(key);
        }

        /// <inheritdoc/>
        public bool Remove(KeyValuePair<TKey, TValue> item)
        {
            return ((ICollection<KeyValuePair<TKey, TValue>>)InnerDictionary).Remove(item);
        }

        /// <inheritdoc/>
        public bool TryGetValue(TKey key, [MaybeNullWhen(false)] out TValue value)
        {
            return ((IDictionary<TKey, TValue>)InnerDictionary).TryGetValue(key, out value);
        }

        /// <inheritdoc/>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable)InnerDictionary).GetEnumerator();
        }
    }
}
