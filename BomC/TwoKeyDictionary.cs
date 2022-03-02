
using System;
using System.Collections.Generic;

namespace TwoKeyDictionary
	{

	/// <summary>
	/// Two keys Dictionary
	/// </summary>
	/// <typeparam name="TK1">First key typ</typeparam>
	/// <typeparam name="TK2">Second key typ</typeparam>
	/// <typeparam name="Tval">Value type</typeparam>
	public class TwoKeyDictionary<TK1, TK2, Tval> where TK1 : IEquatable<TK1> where TK2 : IEquatable<TK2>
		{
		private Dictionary<Tuple<TK1, TK2>, Tval> _dict;
		
		List<TK1> keys1;		// Liste delle chiavi
		List<TK2> keys2;

		/// <summary>
		/// Ctor
		/// </summary>
		public TwoKeyDictionary()
			{
			_dict = new Dictionary<Tuple<TK1, TK2>, Tval>();
			keys1 = new List<TK1>();
			keys2 = new List<TK2>();
			}

		// Two keys are handled by a Tuple<TK1,TK2>
		// Two distinct objects from 'new Tuple<TK1,TK2>(a,b)' would be different, but
		// Tuple<> implements bool Equals(object obj) with a comparer on tuples item1 and item2
		// and GetHashCode() combining the two GetHashCode on item1 and item2

		/// <summary>
		/// Indexer
		/// </summary>
		/// <param name="key1">First key</param>
		/// <param name="key2">Second key</param>
		/// <returns></returns>
		/// <exception cref="KeyNotFoundException">Dictionary entry missing</exception>
		/// <exception cref="ArgumentException">Dictionary entry already existing</exception>
		public Tval this[TK1 key1, TK2 key2]
			{
			get
				{
				Tval tval = default(Tval);
				if(!_dict.TryGetValue(new Tuple<TK1, TK2>(key1, key2), out tval))
					{
					throw new KeyNotFoundException();
					}
				return tval;
				}
			set
				{
				Tuple<TK1, TK2> key = new Tuple<TK1, TK2>(key1, key2);
				if(_dict.ContainsKey(key))
					{
					_dict[key] = value;
					}
				else
					{
					if(!keys1.Contains(key1))
						{
						keys1.Add(key1);
						}
					if(!keys2.Contains(key2))
						{
						keys2.Add(key2);
						}
					_dict.Add(key, value);
					}
				}
			}

		/// <summary>
		/// Dictionary Clear()
		/// </summary>
		public void Clear()
			{
			_dict.Clear();
			}

		/// <summary>
		/// Dictionary ContainsKey(...)
		/// </summary>
		/// <param name="key1">TK1 key1</param>
		/// <param name="key2">TK2 key2</param>
		/// <returns></returns>
		public bool ContainsKey(TK1 key1, TK2 key2)
			{
			return _dict.ContainsKey(new Tuple<TK1, TK2>(key1, key2));
			}

		/// <summary>
		/// Dictionary ContainsValue(...)
		/// </summary>
		/// <param name="value">Tval value</param>
		/// <returns></returns>
		public bool ContainsValue(Tval value)
			{
			return _dict.ContainsValue(value);
			}

		/// <summary>
		/// Dictionary Remove(...)
		/// </summary>
		/// <param name="key1"></param>
		/// <param name="key2"></param>
		/// <returns></returns>
		public bool Remove(TK1 key1, TK2 key2)
			{
			bool ok = true;
			// Rimuove chiavi e valore. ok = false se una delle rimozioni fallisce
			ok = ok && keys1.Remove(key1);
			ok = ok && keys2.Remove(key2);
			ok = ok && _dict.Remove(new Tuple<TK1, TK2>(key1,key2));
			return ok;
			}

		/// <summary>
		/// Enumeratore delle prime chiavi
		/// </summary>
		/// <returns>TK1</returns>
		public IEnumerable<TK1> Keys1()
			{
			foreach(TK1 key in keys1)
				yield return key;
			yield break;
			}
		/// <summary>
		/// Enumeratore delle prime chiavi
		/// </summary>
		/// <returns>TK2</returns>
		public IEnumerable<TK2> Keys2()
			{
			foreach(TK2 key in keys2)
				yield return key;
			yield break;
			}
		
		/// <summary>
		/// Numbers of dictionary keys
		/// </summary>
		/// <returns>Tuple<int,int></returns>
		public Tuple<int,int> Size()
			{
			return new Tuple<int,int>(keys1.Count, keys2.Count);
			}
		}
	}
