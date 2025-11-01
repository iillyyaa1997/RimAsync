using System;
using System.Threading;
using System.Threading.Tasks;
using RimAsync.Core;
using RimAsync.Utils;
using Verse;

namespace RimAsync.Threading
{
    /// <summary>
    /// Manages all asynchronous operations for RimAsync
    /// Handles AsyncTime compatibility and execution mode switching
    /// </summary>
    public static class AsyncManager
    {
        private static bool _initialized = false;
        private static SemaphoreSlim _asyncSemaphore;
        private static CancellationTokenSource _cancellationTokenSource;

        /// <summary>
        /// Global cancellation token for all async operations
        /// </summary>
        public static CancellationToken GlobalCancellationToken => _cancellationTokenSource?.Token ?? CancellationToken.None;

        /// <summary>
        /// Initialize the async manager
        /// </summary>
        public static void Initialize()
        {
            if (_initialized) return;

            try
            {
                Log.Message("[RimAsync] Initializing async manager...");

                // Get thread limit: auto-calculated or manual
                var settings = RimAsyncMod.Settings;
                int maxThreads;

                if (settings != null && settings.enableAutoThreadLimits)
                {
                    maxThreads = RimAsync.Utils.ThreadLimitCalculator.CalculateOptimalThreadLimit();
                    Log.Message($"[RimAsync] Using auto thread limit: {maxThreads}");
                }
                else
                {
                    maxThreads = settings?.maxAsyncThreads ?? 2;
                    Log.Message($"[RimAsync] Using manual thread limit: {maxThreads}");
                }

                _asyncSemaphore = new SemaphoreSlim(maxThreads, maxThreads);
                _cancellationTokenSource = new CancellationTokenSource();

                _initialized = true;
                Log.Message($"[RimAsync] Async manager initialized with {maxThreads} max threads");
            }
            catch (Exception ex)
            {
                Log.Error($"[RimAsync] Failed to initialize async manager: {ex}");
                throw;
            }
        }

        private static void EnsureInitializedLazily()
        {
            if (_initialized) return;
            // Lazy initialization path for test and tool environments without full mod setup
            try
            {
                _asyncSemaphore = new SemaphoreSlim(2, 2);
                _cancellationTokenSource = new CancellationTokenSource();
                _initialized = true;
                Log.Message("[RimAsync] Async manager lazily initialized with default settings");
            }
            catch (Exception ex)
            {
                Log.Error($"[RimAsync] Lazy initialization failed: {ex}");
            }
        }

        /// <summary>
        /// Called when settings change to update thread limits
        /// </summary>
        public static void OnSettingsChanged()
        {
            if (!_initialized) return;

            try
            {
                var settings = RimAsyncMod.Settings;
                int newMaxThreads;

                if (settings != null && settings.enableAutoThreadLimits)
                {
                    newMaxThreads = RimAsync.Utils.ThreadLimitCalculator.CalculateOptimalThreadLimit();
                    Log.Message($"[RimAsync] Auto-calculating thread limit: {newMaxThreads}");
                }
                else
                {
                    newMaxThreads = settings?.maxAsyncThreads ?? 2;
                    Log.Message($"[RimAsync] Using manual thread limit: {newMaxThreads}");
                }

                // Recreate semaphore with new limit
                _asyncSemaphore?.Dispose();
                _asyncSemaphore = new SemaphoreSlim(newMaxThreads, newMaxThreads);

                Log.Message($"[RimAsync] Updated max async threads to {newMaxThreads}");
            }
            catch (Exception ex)
            {
                Log.Error($"[RimAsync] Error updating async manager settings: {ex}");
            }
        }

        /// <summary>
        /// Shutdown the async manager and cancel all operations
        /// </summary>
        public static void Shutdown()
        {
            if (!_initialized) return;

            try
            {
                Log.Message("[RimAsync] Shutting down async manager...");

                // Cancel all running operations
                _cancellationTokenSource?.Cancel();

                // Clean up resources
                _asyncSemaphore?.Dispose();
                _cancellationTokenSource?.Dispose();

                _initialized = false;
                Log.Message("[RimAsync] Async manager shut down successfully");
            }
            catch (Exception ex)
            {
                Log.Error($"[RimAsync] Error during async manager shutdown: {ex}");
            }
        }

        /// <summary>
        /// Execute an operation with automatic async/sync switching based on multiplayer status
        /// </summary>
        /// <param name="asyncOperation">Async version of the operation</param>
        /// <param name="syncOperation">Sync fallback version</param>
        /// <param name="operationName">Name for logging</param>
        public static async Task ExecuteAdaptive(
            Func<CancellationToken, Task> asyncOperation,
            Action syncOperation,
            string operationName = "Unknown")
        {
            if (!_initialized)
            {
                // For test environments, create a minimal default initialization to enable async
                EnsureInitializedLazily();
            }

            var executionMode = RimAsyncCore.GetExecutionMode();

            switch (executionMode)
            {
                case ExecutionMode.FullAsync:
                    // Single player - full async (with fallback on failure for robustness in tests)
                    try
                    {
                        await ExecuteAsync(asyncOperation, operationName, swallowExceptions: false);
                    }
                    catch
                    {
                        syncOperation();
                    }
                    break;

                case ExecutionMode.AsyncTimeEnabled:
                    // Multiplayer with AsyncTime - limited async
                    await ExecuteAsyncTimeCompatible(asyncOperation, syncOperation, operationName);
                    break;

                case ExecutionMode.FullSync:
                    // Multiplayer without AsyncTime - sync only
                    syncOperation();
                    break;

                default:
                    Log.Warning($"[RimAsync] Unknown execution mode {executionMode}, falling back to sync");
                    syncOperation();
                    break;
            }
        }

        /// <summary>
        /// Execute operation in full async mode (single player)
        /// </summary>
        private static async Task ExecuteAsync(Func<CancellationToken, Task> operation, string operationName, bool swallowExceptions = true)
        {
            await _asyncSemaphore.WaitAsync(GlobalCancellationToken);

            try
            {
                var timeoutSeconds = RimAsyncMod.Instance != null
                    ? (RimAsyncMod.Settings?.asyncTimeoutSeconds ?? 5.0f)
                    : 0.1f;
                var timeout = TimeSpan.FromSeconds(timeoutSeconds);
                using var timeoutCts = CancellationTokenSource.CreateLinkedTokenSource(GlobalCancellationToken);
                timeoutCts.CancelAfter(timeout);

                await operation(timeoutCts.Token);

                if (RimAsyncMod.Instance != null && RimAsyncMod.Settings?.enableDebugLogging == true)
                {
                    Log.Message($"[RimAsync] Completed async operation: {operationName}");
                }
            }
            catch (OperationCanceledException oce)
            {
                Log.Warning($"[RimAsync] Async operation cancelled or timed out: {operationName}");
                if (!swallowExceptions)
                {
                    throw;
                }
            }
            catch (Exception ex)
            {
                Log.Error($"[RimAsync] Error in async operation {operationName}: {ex}");
                if (!swallowExceptions)
                {
                    throw;
                }
            }
            finally
            {
                _asyncSemaphore.Release();
            }
        }

        /// <summary>
        /// Execute operation with AsyncTime compatibility (multiplayer with AsyncTime enabled)
        /// </summary>
        private static async Task ExecuteAsyncTimeCompatible(
            Func<CancellationToken, Task> asyncOperation,
            Action syncOperation,
            string operationName)
        {
            // AsyncTime allows limited async operations
            // We can use async for non-game-state affecting operations
            try
            {
                await ExecuteAsync(asyncOperation, operationName, swallowExceptions: false);
            }
            catch (Exception ex)
            {
                Log.Warning($"[RimAsync] AsyncTime operation failed, falling back to sync for {operationName}: {ex.Message}");
                syncOperation();
            }
        }

        /// <summary>
        /// Check if async operations are currently available
        /// </summary>
        public static bool CanExecuteAsync()
        {
            if (!_initialized) return false;
            if (GlobalCancellationToken.IsCancellationRequested) return false;

            return RimAsyncCore.CanUseAsync();
        }

        /// <summary>
        /// Get current async manager status for debugging
        /// </summary>
        public static string GetStatus()
        {
            if (!_initialized) return "Not initialized";

            var mode = RimAsyncCore.GetExecutionMode();
            var available = _asyncSemaphore?.CurrentCount ?? 0;
            var total = RimAsyncMod.Settings?.maxAsyncThreads ?? 2;

            return $"Mode: {mode}, Threads: {available}/{total}, Cancelled: {GlobalCancellationToken.IsCancellationRequested}";
        }
    }
}
