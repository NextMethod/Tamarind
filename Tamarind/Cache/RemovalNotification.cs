using System;
using System.Collections.Generic;
using System.Linq;

namespace Tamarind.Cache
{
	/// <summary>
	///     A notification of the removal of a single entry. The key and/or value may be null if they were already garbage
	///     collected.
	///     <para>
	///         This class holds strong references to the key and value, regardless of the type of references the cache may
	///         be using.
	///     </para>
	/// </summary>
	public struct RemovalNotification<TKey, TValue> : IEquatable<RemovalNotification<TKey, TValue>>
	{

		private readonly RemovalCause _cause;

		private readonly TKey _key;

		private readonly TValue _value;

		private readonly bool _wasEvicted;

		internal RemovalNotification(TKey key, TValue value, RemovalCause cause)
		{
			_key = key;
			_value = value;
			_cause = cause;
			_wasEvicted = cause != RemovalCause.Explicit && cause != RemovalCause.Replaced;
		}

		public TKey Key
		{
			get { return _key; }
		}

		public TValue Value
		{
			get { return _value; }
		}

		/// <summary>
		///     The cause for which the entry was removed.
		/// </summary>
		public RemovalCause Cause
		{
			get { return _cause; }
		}


		/// <summary>
		///     <c>true</c> if there was an automatic removal due to eviction (the cause is neither
		///     <see cref="RemovalCause.Explicit" /> nor <see cref="RemovalCause.Replaced" />).
		/// </summary>
		public bool WasEvicted
		{
			get { return _wasEvicted; }
		}


		public bool Equals(RemovalNotification<TKey, TValue> other)
		{
			return EqualityComparer<TKey>.Default.Equals(_key, other._key) && EqualityComparer<TValue>.Default.Equals(_value, other._value);
		}

		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj)) { return false; }
			return obj is RemovalNotification<TKey, TValue> && Equals((RemovalNotification<TKey, TValue>) obj);
		}

		public override int GetHashCode()
		{
			unchecked { return (EqualityComparer<TKey>.Default.GetHashCode(_key) * 397) ^ EqualityComparer<TValue>.Default.GetHashCode(_value); }
		}

		public override string ToString()
		{
			return string.Format("Key: {0}, Value: {1}", Key, Value);
		}

		public static bool operator ==(RemovalNotification<TKey, TValue> left, RemovalNotification<TKey, TValue> right)
		{
			return left.Equals(right);
		}

		public static bool operator !=(RemovalNotification<TKey, TValue> left, RemovalNotification<TKey, TValue> right)
		{
			return !left.Equals(right);
		}

	}
}
