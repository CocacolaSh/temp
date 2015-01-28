using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.Collections.Concurrent;

namespace Ocean.Communication.Common
{
    /// <summary>
    /// 线程安全的集合
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [Serializable]
    public class SafeCollection<T> : ICollection<T>
    {
        private readonly ConcurrentDictionary<T, bool> inner = new ConcurrentDictionary<T, bool>();

        public void Add(T item)
        {
            this.inner.TryAdd(item, true);
        }

        public void Clear()
        {
            this.inner.Clear();
        }

        public bool Contains(T item)
        {
            return this.inner.ContainsKey(item);
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            this.inner.Keys.CopyTo(array, arrayIndex);
        }

        public int Count
        {
            get
            {
                return this.inner.Count;
            }
        }

        public bool IsReadOnly
        {
            get { return false; }
        }

        public bool Remove(T item)
        {
            bool value;
            return this.inner.TryRemove(item, out value);
        }

        public IEnumerator<T> GetEnumerator()
        {
            return this.inner.Keys.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}