using System;
using System.Collections;
using System.Collections.Generic;

namespace GooglePlayPublisherCli
{
	public class SingletonList<T> : IList<T>
	{
		private T _value;

		public SingletonList(T value)
		{
			if (value == null)
			{
				throw new ArgumentException(nameof(value));
			}

			_value = value;
		}

		#region ICollection<T> Properties

		public int Count { get; } = 1;

		public bool IsReadOnly { get; } = true;

		#endregion

		#region IList<T> Members

		public IEnumerator<T> GetEnumerator()
		{
			yield return _value;
		}

		IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

		public void Add(T item) => throw new NotSupportedException("Adding is not supported.");

		public void Clear() => throw new NotSupportedException("Clearing is not supported.");

		public bool Contains(T item)
		{
			if (item == null)
			{
				throw new ArgumentException(nameof(item));
			}

			return item.Equals(_value);
		}

		public void CopyTo(T[] array, int arrayIndex)
		{
			if (array == null)
			{
				throw new ArgumentException(nameof(array));
			}

			array[arrayIndex] = _value;
		}

		public bool Remove(T item) => throw new NotSupportedException("Removing is not supported.");

		public int IndexOf(T item) => Contains(item) ? 0 : throw new IndexOutOfRangeException();

		public void Insert(int index, T item) => throw new NotSupportedException("Inserting is not supported.");

		public void RemoveAt(int index) => throw new NotSupportedException("Removing is not supported.");

		public T this[int index]
		{
			get => index == 0 ? _value : throw new IndexOutOfRangeException();
			set
			{
				if (index == 0)
				{
					_value = value;
				}
				else
				{
					throw new IndexOutOfRangeException();
				}
			}
		}

		#endregion
	}
}