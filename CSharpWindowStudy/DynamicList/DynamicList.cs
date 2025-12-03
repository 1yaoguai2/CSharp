using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DynamicList
{
    public class DynamicList<T> : IEnumerable<T>
    {
        private T[] _items;  //内部存储列表
        private int _count;      //当前元素数量
        private int _capacity;   //容量

        public DynamicList() : this(4) { }

        //构造函数
        public DynamicList(int n)
        {
            if (n < 0) throw new ArgumentOutOfRangeException(nameof(n), "初始容量不能为负数");
            _capacity = n;
            _items = new T[n];
            _count = 0;
        }

        //属性，当前元素数量
        public int Coun => _count;
        //属性，当前元素容量
        public int Capacity => _capacity;

        //索引器：通过索引访问元素
        public T this[int index]
        {
            get
            {
                if (index < 0 || index >= _count)
                    throw new ArgumentOutOfRangeException("索引超出范围");
                return _items[index];
            }
            set
            {
                if (index < 0 || index >= _count)
                    throw new ArgumentOutOfRangeException("索引超出范围");
                _items[index] = value;
            }
        }

        //添加元素
        public void Add(T item)
        {
            //容量不足，自动扩展（翻倍）
            if (_count == _capacity)
                Resize(_capacity == 0 ? 4 : _capacity * 2);

            _items[_count] = item;
            _count++;
        }

        //移除元素（返回是否成功）
        public bool Remove(T item)
        {
            int index = IndexOf(item);
            if (index == -1)
                return false;
            RemoveAt(index);
            return true;
        }

        //按元素编号移除
        private void RemoveAt(int index)
        {
            if (index < 0 || index >= _count)
                throw new ArgumentOutOfRangeException("超出索引范围");
            //从索引位置开始，后续元素往前移动一位
            for (int i = index; i < _count - 1; i++)
                _items[i] = _items[i + 1];

            _count--;
            //清空最后一个位置（避免引用残留）
            _items[_count - 1] = default;
        }

        //获取元素索引，无该元素则返回-1
        private int IndexOf(T? item)
        {
            for (int i = 0; i < _count; i++)
            {
                if (EqualityComparer<T>.Default.Equals(_items[i], item))
                    return i;
            }
            return -1;
        }

        //检查是否包含元素
        public bool Contains(T item) => IndexOf(item) != -1;

        //清空
        public void Clear()
        {
            for (int i = 0; i < _count; i++)
                _items[i] = default;
            _count = 0;
        }

        /// <summary>
        /// 清空，并重置容量
        /// </summary>
        /// <param name="newCapacity">新容量</param>
        public void Clear(int newCapacity)
        {
            for (int i = 0; i < _count; i++)
                _items[i] = default;
            _count = 0;
            _items = new T[newCapacity];
            _capacity = newCapacity;
        }

        //修改容量
        private void Resize(int newCapacity)
        {
            if (newCapacity <= _count)
                return;

            T[] newItems = new T[newCapacity];
            Array.Copy(_items, newItems, _count);
            _items = newItems;
            _capacity = newCapacity;
        }

        //失效IEnumberator<T> 支持foreach遍历
        public IEnumerator<T> GetEnumerator()
        {
            for (int i = 0; i < _count; i++)
                yield return _items[i];
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        //示例
        public void Sort() where T : IComparable<T>
        {
            for (int i = 0; i < _count - 1; i++)
            {
                for (int j = 0; j < _count - 1; j++)
                {
                    if (_items[j].CompareTo(_items[j + 1]) > 0)
                    {
                        T temp = _items[j];
                        _items[j] = _items[j + 1];
                        _item[j + 1] = temp;
                        (_items[j], _items[j + 1]) = (_items[j + 1], _items[j]);
                    }
                }
            }
        }
    }
}
