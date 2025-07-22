using System;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace RimAsync.Tests.Mocks
{
    /// <summary>
    /// Mock implementation of AsyncManager for testing
    /// </summary>
    public class MockAsyncManager : IDisposable
    {
        private readonly List<Task> _activeTasks = new List<Task>();
        private readonly CancellationTokenSource _cancellationSource = new CancellationTokenSource();
        private bool _disposed = false;
        
        // Mock properties
        public bool IsAsyncTimeEnabled { get; set; } = true;
        public int MaxConcurrentTasks { get; set; } = 4;
        public int ActiveTaskCount => _activeTasks.Count;
        public bool IsShutdownRequested { get; private set; }
        
        // Statistics for testing
        public int TasksExecuted { get; private set; }
        public int TasksCancelled { get; private set; }
        public int TasksFailed { get; private set; }
        
        /// <summary>
        /// Mock async task execution
        /// </summary>
        public async Task<T> ExecuteAsync<T>(Func<CancellationToken, Task<T>> task, CancellationToken cancellationToken = default)
        {
            if (_disposed)
                throw new ObjectDisposedException(nameof(MockAsyncManager));
                
            if (!IsAsyncTimeEnabled)
            {
                // Fallback to sync execution
                return await task(cancellationToken);
            }
            
            var combinedToken = CancellationTokenSource.CreateLinkedTokenSource(
                _cancellationSource.Token, cancellationToken).Token;
            
            var executeTask = Task.Run(async () =>
            {
                try
                {
                    var result = await task(combinedToken);
                    TasksExecuted++;
                    return result;
                }
                catch (OperationCanceledException)
                {
                    TasksCancelled++;
                    throw;
                }
                catch
                {
                    TasksFailed++;
                    throw;
                }
            }, combinedToken);
            
            lock (_activeTasks)
            {
                _activeTasks.Add(executeTask);
            }
            
            try
            {
                return await executeTask;
            }
            finally
            {
                lock (_activeTasks)
                {
                    _activeTasks.Remove(executeTask);
                }
            }
        }
        
        /// <summary>
        /// Mock async execution without return value
        /// </summary>
        public async Task ExecuteAsync(Func<CancellationToken, Task> task, CancellationToken cancellationToken = default)
        {
            await ExecuteAsync(async ct =>
            {
                await task(ct);
                return true;
            }, cancellationToken);
        }
        
        /// <summary>
        /// Mock shutdown procedure
        /// </summary>
        public async Task ShutdownAsync(TimeSpan timeout = default)
        {
            IsShutdownRequested = true;
            _cancellationSource.Cancel();
            
            var timeoutSource = timeout == default 
                ? new CancellationTokenSource(TimeSpan.FromSeconds(5))
                : new CancellationTokenSource(timeout);
            
            try
            {
                await Task.WhenAll(_activeTasks).WaitAsync(timeoutSource.Token);
            }
            catch (OperationCanceledException)
            {
                // Timeout occurred - some tasks didn't complete
            }
        }
        
        /// <summary>
        /// Reset mock to initial state
        /// </summary>
        public void Reset()
        {
            TasksExecuted = 0;
            TasksCancelled = 0;
            TasksFailed = 0;
            IsShutdownRequested = false;
            _activeTasks.Clear();
        }
        
        /// <summary>
        /// Simulate performance metrics
        /// </summary>
        public MockPerformanceMetrics GetPerformanceMetrics()
        {
            return new MockPerformanceMetrics
            {
                ThreadUtilization = Math.Min(100.0f, (ActiveTaskCount / (float)MaxConcurrentTasks) * 100),
                TasksPerSecond = TasksExecuted / 10.0f, // Mock calculation
                AverageExecutionTime = TimeSpan.FromMilliseconds(50 + new Random().Next(100)),
                MemoryUsage = GC.GetTotalMemory(false)
            };
        }
        
        public void Dispose()
        {
            if (!_disposed)
            {
                _cancellationSource?.Cancel();
                _cancellationSource?.Dispose();
                _disposed = true;
            }
        }
    }
    
    /// <summary>
    /// Mock performance metrics for testing
    /// </summary>
    public class MockPerformanceMetrics
    {
        public float ThreadUtilization { get; set; }
        public float TasksPerSecond { get; set; }
        public TimeSpan AverageExecutionTime { get; set; }
        public long MemoryUsage { get; set; }
        
        public override string ToString()
        {
            return $"Thread Utilization: {ThreadUtilization:F1}%, " +
                   $"Tasks/sec: {TasksPerSecond:F1}, " +
                   $"Avg Time: {AverageExecutionTime.TotalMilliseconds:F0}ms, " +
                   $"Memory: {MemoryUsage / 1024 / 1024}MB";
        }
    }
} 