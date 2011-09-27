﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CountType = System.Int32;
using System.Linq.Expressions;

namespace CSharpUtils.Containers.RedBlackTree
{
	public partial class RedBlackTreeWithStats<TElement>
	{
		public class Range : IEnumerable<TElement>, ICollection<TElement>, ICloneable, IOrderedQueryable<TElement>
		{
			internal RedBlackTreeWithStats<TElement> ParentTree;
			internal Node _rbegin;
			internal Node _rend;
			internal CountType _rbeginPosition;
			internal CountType _rendPosition;
		
			public CountType GetOffsetPosition(CountType index) {
				if (_rbeginPosition == -1) {
					return ParentTree.getNodePosition(_rbegin) + index;
				}
				return _rbeginPosition + index;
			}
		
			internal Range(RedBlackTreeWithStats<TElement> ParentTree, Node b, Node e, CountType rbeginPosition = -1, CountType rendPosition = -1) {
				this.ParentTree = ParentTree;
				if (b == null) b = ParentTree.locateNodeAtPosition(rbeginPosition);
				if (e == null) e = ParentTree.locateNodeAtPosition(rendPosition);
				if (b == null || e == null) {
					//b = _end.left;
					//e = _end.left;
					b = e = ParentTree.RootNode;
					rbeginPosition = -1;
					rendPosition = -1;
				}
				_rbegin = b;
				_rend = e;
				_rbeginPosition = rbeginPosition;
				_rendPosition = rendPosition;
			}
		
			public Range Clone() {
				return new Range(ParentTree, _rbegin, _rend, _rbeginPosition, _rendPosition);
			}
		
			public Range Limit(CountType limitCount) {
				Assert(limitCount >= 0);
			
				if (_rbeginPosition != -1 && _rendPosition != -1) {
					if (_rbeginPosition + limitCount > _rendPosition) {
						limitCount = _rendPosition - _rbeginPosition; 
					}
				} else {
					// Unsecure.
				}
				return LimitUnchecked(limitCount);
			}
		
			public Range LimitUnchecked(CountType limitCount) {
				Assert(limitCount >= 0);

				return new Range(
					ParentTree,
					_rbegin, null,
					_rbeginPosition, GetOffsetPosition(limitCount)
				);
			}
		
			public Range Skip(CountType skipCount)
			{
				return SkipUnchecked(skipCount);
			}

			public Range Take(CountType skipCount)
			{
				return Limit(skipCount);
			}
		
			public Range SkipUnchecked(CountType skipCount)
			{
				return new Range(
					ParentTree,
					null, _rend,
					GetOffsetPosition(skipCount), _rendPosition
				);
			}

			bool IsEmpty
			{
				get {
					return _rbegin == _rend;
				}
			}

			public CountType Length
			{
				get
				{
					//writefln("Begin: %d:%s", countLesser(_begin), *_begin);
					//writefln("End: %d:%s", countLesser(_end), *_end);
					//return _begin

					if (_rbeginPosition != -1 && _rendPosition != -1)
					{
						return _rendPosition - _rbeginPosition;
					}

					return ParentTree.countLesser(_rend) - ParentTree.countLesser(_rbegin);
				}
			}

			public Range Slice()
			{
				return this.Clone();
			}

			public Range Slice(CountType start, CountType end)
			{
				return new Range(
					ParentTree,
					null,
					null,
					GetOffsetPosition(start),
					GetOffsetPosition(end)
				);
			}

			public Range Slice(CountType start)
			{
				return new Range(
					ParentTree,
					null,
					null,
					GetOffsetPosition(start),
					Length
				);
			}

			public Node this[CountType Index]
			{
				get
				{
					return ParentTree.locateNodeAtPosition(GetOffsetPosition(Index));
				}
			}

			/*
			Type front
			{
				get
				{
					return _rbegin.value;
				}
			}

			Type back
			{
				get
				{
					return _rend.prev.value;
				}
			}

			void popFront()
			{
				_rbegin = _rbegin.next;
			}
		
			void popBack()
			{
				_rend = _rend.prev;
			}
			*/

			IEnumerator<TElement> IEnumerable<TElement>.GetEnumerator()
			{
				for (Node current = _rbegin; current != _rend; current = current.next)
				{
					yield return current.Value;
				}
			}

			System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
			{
				for (Node current = _rbegin; current != _rend; current = current.next)
				{
					yield return current.Value;
				}
			}

			public void Add(TElement item)
			{
				throw new NotImplementedException();
			}

			public void Clear()
			{
				throw new NotImplementedException();
			}

			public bool Contains(TElement item)
			{
				if (IsEmpty) return false;
				//Node node = ParentTree._find(item);
				if (ParentTree.Comparer.Compare(item, _rbegin.value) < 0) return false;
				if (ParentTree.Comparer.Compare(item, _rend.value) > 0) return false;
				return true;
			}

			public void CopyTo(TElement[] array, CountType arrayIndex)
			{
				throw new NotImplementedException();
			}

			public CountType Count
			{
				get { return Length; }
			}

			public bool IsReadOnly
			{
				get { return true; }
			}

			public bool Remove(TElement item)
			{
				throw new NotImplementedException();
			}

			object ICloneable.Clone()
			{
				return new Range(ParentTree, _rbegin, _rend, _rbeginPosition, _rendPosition);
			}

			public System.Type ElementType
			{
				get { return typeof(TElement); }
			}

			public Expression Expression
			{
				get { return Expression.Constant(this); }
			}

			public IQueryProvider Provider
			{
				get { return new RedBlackTreeWithStatsQueryProvider<TElement>(this); }
			}
		}
	}
}