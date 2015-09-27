using System;
using System.Collections.Generic;
namespace NeinTom
{
    
	public class CircularBuffer<T>:IList<T>,IEnumerable<T>
	{
        public delegate void ItemThrownDelegate(object sender, T thrown);
        public event ItemThrownDelegate ItemThrown;
        private T[] buffer;
		private int currentIndex;
		private int count;
		public CircularBuffer(int capacity)
		{
			buffer = new T[capacity];
			count = 0;
			currentIndex = 0;
		}
		private int getRealIndex(int index)
		{
			int realIndex = currentIndex - index - 1;
			if (currentIndex < count) {
				if (currentIndex + Math.Abs (realIndex) >= count)
					return -1;
				if (realIndex < 0)
					realIndex += buffer.Length;
			}
			if (realIndex < 0)
				return -1;
			return realIndex;
		}
		public void Add(T item)
		{
			if (buffer[currentIndex] != null)
				ItemThrown(this,buffer[currentIndex]);
			buffer[currentIndex++] = item;

            currentIndex = currentIndex % buffer.Length;
            if (count < buffer.Length)
			    count++;
		}
		public void Clear()
		{
			for (int i = 0; i < buffer.Length; i++)
				buffer [i] = default(T);
			count = 0;
			currentIndex = 0;
		}

		public bool Contains(T item)
		{
			foreach(T it in this)
			{
				if (it.Equals(item))
					return true;
			}
			return false;
		}
		public int IndexOf(T item)
		{
			int index = 0;
			foreach(T it in this)
			{
				if (it.Equals(item))
					return index;
				index++;
			}
			return -1;
		}
		public void Insert(int index,T item)
		{
			int realIndex = getRealIndex (index);
			if (realIndex < 0)
				throw new IndexOutOfRangeException();
			if (buffer [realIndex].Equals(default(T)))
				ItemThrown (this,buffer [realIndex]);//TODO: really throw out?
			buffer [realIndex] = item;
		}
		public bool IsFixedSize
		{
			get{
				return true;
			}
		}
		public bool IsReadOnly{
			get{
				return false;
			}
		}
		public bool Remove(T item)
		{
			int index = IndexOf (item);
			if (index >= 0)
				RemoveAt (index);
			else
				return false;
			return true;
		}
		public void RemoveAt(int index)
		{
			int realIndex = getRealIndex (index);
			if (realIndex < 0)
				throw new IndexOutOfRangeException ();
			buffer [realIndex] = default(T);//TODO: shifting elements

		}
		public void CopyTo(T[] array,int index)
		{
			foreach(T it in this)
			{
				array [index] = it;
				index++;
			}
		}
		public T this[int index]
		{
			get{
				int realIndex = getRealIndex (index);
				if (realIndex < 0)
					throw new IndexOutOfRangeException ();
				return buffer [realIndex];
			}
			set{
				int realIndex = getRealIndex (index);
				if (realIndex < 0)
					throw new IndexOutOfRangeException ();
				buffer [realIndex] = value;
			}
		}
		public int Count{
			get{
				return count;
			}
		}
		public IEnumerator<T> GetEnumerator()
		{
			return (IEnumerator<T>)new CircularEnumerator<T>(buffer,currentIndex,count);
		}
		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return (System.Collections.IEnumerator)new CircularEnumerator<T>(buffer,currentIndex,count);
		}
	}
	public class CircularEnumerator<T>:IEnumerator<T>//NO circular enumeration
	{
		private T[] buffer;
		private int currentIndex,count,index,moved;
		public CircularEnumerator(T[] buffer,int currentIndex,int count)
		{
			this.buffer = buffer;
			this.currentIndex = currentIndex;
			this.index = currentIndex;
			this.count = count;
			this.moved = 0;
		}
		public bool MoveNext()
		{
			if (count == 0)
				return false;
			index--;
			moved++;
			if (count > currentIndex)
			{
				if (index < 0)
					index = buffer.Length - 1;
				
			}
			/*if (buffer [index].Equals(default(T)))
				return MoveNext ();//TODO:verify*/
			return moved <= count;
		}
		public void Reset()
		{
			index = currentIndex;
			moved = 0;
		}
		object System.Collections.IEnumerator.Current
		{
			get
			{
				return buffer[index];
			}
		}
		public T Current
		{
			get
			{
				return buffer[index];
			}
		}
		public void Dispose()//TODO:
		{
		}
	}
}

