using System;
using System.Collections.Generic;
using System.IO;

namespace RimAsync.Tests.Mocks
{
    /// <summary>
    /// Mock implementation of MultiplayerCompat for testing
    /// </summary>
    public static class MockMultiplayerCompat
    {
        private static bool _isMultiplayerActive = false;
        private static bool _isAsyncTimeEnabled = false;
        private static readonly List<string> _mockDesyncLogs = new List<string>();
        
        // Mock state
        public static bool IsMultiplayerActive 
        { 
            get => _isMultiplayerActive;
            set => _isMultiplayerActive = value;
        }
        
        public static bool IsAsyncTimeEnabled 
        { 
            get => _isAsyncTimeEnabled;
            set => _isAsyncTimeEnabled = value;
        }
        
        // Mock events
        public static event Action<bool> OnAsyncTimeStateChanged;
        public static event Action<string> OnDesyncDetected;
        
        /// <summary>
        /// Mock AsyncTime detection
        /// </summary>
        public static bool DetectAsyncTime()
        {
            // Simulate reflection-based detection
            return IsAsyncTimeEnabled;
        }
        
        /// <summary>
        /// Mock safe async execution wrapper
        /// </summary>
        public static T SafeExecute<T>(Func<T> syncAction, Func<T> asyncAction)
        {
            if (IsMultiplayerActive && !IsAsyncTimeEnabled)
            {
                // Multiplayer without AsyncTime - use sync
                return syncAction();
            }
            else
            {
                // Single player or AsyncTime enabled - use async
                return asyncAction();
            }
        }
        
        /// <summary>
        /// Mock deterministic execution check
        /// </summary>
        public static bool IsDeterministicExecution()
        {
            // In multiplayer, execution must be deterministic
            return IsMultiplayerActive;
        }
        
        /// <summary>
        /// Mock desync log creation
        /// </summary>
        public static void SimulateDesync(string reason)
        {
            var desyncEntry = $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] DESYNC: {reason}";
            _mockDesyncLogs.Add(desyncEntry);
            
            // Simulate writing to MpDesyncs folder
            var mockLogPath = Path.Combine(TestConfig.MpDesyncPath, $"desync_{DateTime.Now:yyyyMMdd_HHmmss}.log");
            
            OnDesyncDetected?.Invoke(desyncEntry);
        }
        
        /// <summary>
        /// Check if any desyncs occurred during test
        /// </summary>
        public static bool HasDesyncsOccurred()
        {
            return _mockDesyncLogs.Count > 0;
        }
        
        /// <summary>
        /// Get all mock desync logs
        /// </summary>
        public static IReadOnlyList<string> GetDesyncLogs()
        {
            return _mockDesyncLogs.AsReadOnly();
        }
        
        /// <summary>
        /// Simulate AsyncTime state change
        /// </summary>
        public static void ChangeAsyncTimeState(bool enabled)
        {
            var previousState = IsAsyncTimeEnabled;
            IsAsyncTimeEnabled = enabled;
            
            if (previousState != enabled)
            {
                OnAsyncTimeStateChanged?.Invoke(enabled);
            }
        }
        
        /// <summary>
        /// Simulate multiplayer session start
        /// </summary>
        public static void StartMultiplayerSession(bool asyncTimeEnabled = false)
        {
            IsMultiplayerActive = true;
            IsAsyncTimeEnabled = asyncTimeEnabled;
            ClearDesyncLogs();
        }
        
        /// <summary>
        /// Simulate multiplayer session end
        /// </summary>
        public static void EndMultiplayerSession()
        {
            IsMultiplayerActive = false;
            IsAsyncTimeEnabled = false;
        }
        
        /// <summary>
        /// Clear all mock desync logs
        /// </summary>
        public static void ClearDesyncLogs()
        {
            _mockDesyncLogs.Clear();
        }
        
        /// <summary>
        /// Reset mock to clean state
        /// </summary>
        public static void Reset()
        {
            IsMultiplayerActive = false;
            IsAsyncTimeEnabled = false;
            ClearDesyncLogs();
        }
        
        /// <summary>
        /// Mock version compatibility check
        /// </summary>
        public static bool IsVersionCompatible(string requiredVersion)
        {
            // Always return true in mock for testing
            return true;
        }
        
        /// <summary>
        /// Mock network sync validation
        /// </summary>
        public static bool ValidateNetworkSync(object gameState)
        {
            if (!IsMultiplayerActive)
                return true;
                
            // Simulate checksum validation
            var checksum = gameState?.GetHashCode() ?? 0;
            
            // Simulate occasional desync for testing
            if (checksum % 1000 == 0) // 0.1% chance
            {
                SimulateDesync($"Network sync validation failed, checksum: {checksum}");
                return false;
            }
            
            return true;
        }
        
        /// <summary>
        /// Get mock multiplayer statistics
        /// </summary>
        public static MockMultiplayerStats GetStats()
        {
            return new MockMultiplayerStats
            {
                IsActive = IsMultiplayerActive,
                AsyncTimeEnabled = IsAsyncTimeEnabled,
                DesyncCount = _mockDesyncLogs.Count,
                SessionDuration = TimeSpan.FromMinutes(15), // Mock duration
                PlayerCount = IsMultiplayerActive ? 2 : 1
            };
        }
    }
    
    /// <summary>
    /// Mock multiplayer statistics
    /// </summary>
    public class MockMultiplayerStats
    {
        public bool IsActive { get; set; }
        public bool AsyncTimeEnabled { get; set; }
        public int DesyncCount { get; set; }
        public TimeSpan SessionDuration { get; set; }
        public int PlayerCount { get; set; }
        
        public override string ToString()
        {
            return $"MP Active: {IsActive}, AsyncTime: {AsyncTimeEnabled}, " +
                   $"Players: {PlayerCount}, Desyncs: {DesyncCount}, " +
                   $"Duration: {SessionDuration:mm\\:ss}";
        }
    }
} 