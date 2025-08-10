using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace RimAsync.Utils
{
    /// <summary>
    /// Thread-safe collections for async operations
    /// Provides safe access to shared data between main thread and async operations
    /// </summary>
    public static class AsyncSafeCollections
    {
        /// <summary>
        /// Thread-safe list wrapper
        /// </summary>
        public class SafeList<T>
        {
            private readonly ConcurrentQueue<T> _items = new ConcurrentQueue<T>();
            private readonly object _lock = new object();

            public void Add(T item)
            {
                _items.Enqueue(item);
            }

            public bool TryTake(out T item)
            {
                return _items.TryDequeue(out item);
            }

            public IEnumerable<T> TakeAll()
            {
                var result = new List<T>();
                while (_items.TryDequeue(out T item))
                {
                    result.Add(item);
                }
                return result;
            }

            public int Count => _items.Count;

            public bool IsEmpty => _items.IsEmpty;

            public T[] ToArray()
            {
                return _items.ToArray();
            }
        }

        /// <summary>
        /// Thread-safe dictionary wrapper
        /// </summary>
        public class SafeDictionary<TKey, TValue>
        {
            private readonly ConcurrentDictionary<TKey, TValue> _dict = new ConcurrentDictionary<TKey, TValue>();

            public TValue GetOrAdd(TKey key, TValue value)
            {
                return _dict.GetOrAdd(key, value);
            }

            public TValue GetOrAdd(TKey key, System.Func<TKey, TValue> factory)
            {
                return _dict.GetOrAdd(key, factory);
            }

            public bool TryGetValue(TKey key, out TValue value)
            {
                return _dict.TryGetValue(key, out value);
            }

            public bool TryAdd(TKey key, TValue value)
            {
                return _dict.TryAdd(key, value);
            }

            public bool TryRemove(TKey key, out TValue value)
            {
                return _dict.TryRemove(key, out value);
            }

            public void Clear()
            {
                _dict.Clear();
            }

            public int Count => _dict.Count;

            public IEnumerable<TKey> Keys => _dict.Keys;

            public IEnumerable<TValue> Values => _dict.Values;
        }

        /// <summary>
        /// Thread-safe set wrapper
        /// </summary>
        public class SafeSet<T>
        {
            private readonly ConcurrentDictionary<T, byte> _set = new ConcurrentDictionary<T, byte>();

            public bool Add(T item)
            {
                return _set.TryAdd(item, 0);
            }

            public bool Remove(T item)
            {
                return _set.TryRemove(item, out _);
            }

            public bool Contains(T item)
            {
                return _set.ContainsKey(item);
            }

            public void Clear()
            {
                _set.Clear();
            }

            public int Count => _set.Count;

            public T[] ToArray()
            {
                return _set.Keys.ToArray();
            }
        }

        /// <summary>
        /// Thread-safe counter
        /// </summary>
        public class SafeCounter
        {
            private long _value = 0;

            public long Value => _value;

            public long Increment()
            {
                return System.Threading.Interlocked.Increment(ref _value);
            }

            public long Decrement()
            {
                return System.Threading.Interlocked.Decrement(ref _value);
            }

            public long Add(long amount)
            {
                return System.Threading.Interlocked.Add(ref _value, amount);
            }

            public void Reset()
            {
                System.Threading.Interlocked.Exchange(ref _value, 0);
            }

            public long Exchange(long newValue)
            {
                return System.Threading.Interlocked.Exchange(ref _value, newValue);
            }
        }

        /// <summary>
        /// Thread-safe flag
        /// </summary>
        public class SafeFlag
        {
            private int _flag = 0;

            public bool IsSet => _flag != 0;

            public bool Set()
            {
                return System.Threading.Interlocked.Exchange(ref _flag, 1) == 0;
            }

            public bool Reset()
            {
                return System.Threading.Interlocked.Exchange(ref _flag, 0) == 1;
            }

            public bool TrySet()
            {
                return System.Threading.Interlocked.CompareExchange(ref _flag, 1, 0) == 0;
            }
        }

        /// <summary>
        /// Thread-safe queue with size limit
        /// </summary>
        public class BoundedSafeQueue<T>
        {
            private readonly ConcurrentQueue<T> _queue = new ConcurrentQueue<T>();
            private readonly int _maxSize;
            private long _currentSize = 0;

            public BoundedSafeQueue(int maxSize)
            {
                _maxSize = maxSize;
            }

            public bool TryEnqueue(T item)
            {
                if (System.Threading.Interlocked.Read(ref _currentSize) >= _maxSize)
                {
                    return false;
                }

                _queue.Enqueue(item);
                System.Threading.Interlocked.Increment(ref _currentSize);
                return true;
            }

            public bool TryDequeue(out T item)
            {
                if (_queue.TryDequeue(out item))
                {
                    System.Threading.Interlocked.Decrement(ref _currentSize);
                    return true;
                }
                return false;
            }

            public int Count => (int)_currentSize;

            public int MaxSize => _maxSize;

            public bool IsFull => _currentSize >= _maxSize;

            public bool IsEmpty => _currentSize == 0;
        }
    }

    /// <summary>
    /// Async-safe communication channels
    /// </summary>
    public static class AsyncChannels
    {
        /// <summary>
        /// Message passed from async operations to main thread
        /// </summary>
        public class AsyncMessage
        {
            public string Type { get; set; }
            public object Data { get; set; }
            public System.DateTime Timestamp { get; set; } = System.DateTime.UtcNow;
        }

        /// <summary>
        /// Channel for async to main thread communication
        /// </summary>
        public static readonly AsyncSafeCollections.SafeList<AsyncMessage> MainThreadMessages =
            new AsyncSafeCollections.SafeList<AsyncMessage>();

        /// <summary>
        /// Send a message from async operation to main thread
        /// </summary>
        public static void SendToMainThread(string type, object data = null)
        {
            MainThreadMessages.Add(new AsyncMessage
            {
                Type = type,
                Data = data
            });
        }

        /// <summary>
        /// Process all pending messages on main thread
        /// </summary>
        public static void ProcessMainThreadMessages()
        {
            var messages = MainThreadMessages.TakeAll();
            foreach (var message in messages)
            {
                try
                {
                    ProcessMainThreadMessage(message);
                }
                catch (System.Exception ex)
                {
                    Verse.Log.Error($"[RimAsync] Error processing async message {message.Type}: {ex}");
                }
            }
        }

        /// <summary>
        /// Process a single message on main thread
        /// </summary>
        private static void ProcessMainThreadMessage(AsyncMessage message)
        {
            switch (message.Type)
            {
                case "CacheInvalidate":
                    if (message.Data is string key)
                    {
                        SmartCache.Invalidate(key);
                    }
                    break;

                case "PerformanceMetric":
                    if (message.Data is (string name, float value))
                    {
                        PerformanceMonitor.RecordMetric(name, value);
                    }
                    break;

                case "Log":
                    if (message.Data is string logMessage)
                    {
                        Verse.Log.Message($"[RimAsync] {logMessage}");
                    }
                    break;

                default:
                    if (RimAsyncMod.Settings?.enableDebugLogging == true)
                    {
                        Verse.Log.Warning($"[RimAsync] Unknown async message type: {message.Type}");
                    }
                    break;
            }
        }
    }
}
