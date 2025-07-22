using System;
using System.Reflection;
using Verse;

namespace RimAsync.Utils
{
    /// <summary>
    /// Handles detection and compatibility with RimWorld Multiplayer mod
    /// Provides safe access to multiplayer state and AsyncTime setting
    /// </summary>
    public static class MultiplayerCompat
    {
        private static bool _initialized = false;
        private static bool _multiplayerLoaded = false;
        private static Assembly _multiplayerAssembly = null;
        
        // Reflected types and methods for multiplayer compatibility
        private static Type _multiplayerAPIType = null;
        private static PropertyInfo _isInMultiplayerProperty = null;
        private static Type _multiplayerSettingsType = null;
        private static PropertyInfo _asyncTimeProperty = null;

        /// <summary>
        /// True if RimWorld Multiplayer mod is loaded and active
        /// </summary>
        public static bool IsMultiplayerLoaded => _multiplayerLoaded;

        /// <summary>
        /// True if currently in a multiplayer session
        /// </summary>
        public static bool IsInMultiplayer
        {
            get
            {
                if (!_multiplayerLoaded || _isInMultiplayerProperty == null) return false;
                
                try
                {
                    return (bool)_isInMultiplayerProperty.GetValue(null);
                }
                catch (Exception ex)
                {
                    Log.Error($"[RimAsync] Error checking multiplayer status: {ex}");
                    return false;
                }
            }
        }

        /// <summary>
        /// True if AsyncTime setting is enabled in multiplayer
        /// </summary>
        public static bool AsyncTimeEnabled
        {
            get
            {
                if (!_multiplayerLoaded || _asyncTimeProperty == null) return false;
                
                try
                {
                    return (bool)_asyncTimeProperty.GetValue(null);
                }
                catch (Exception ex)
                {
                    Log.Error($"[RimAsync] Error checking AsyncTime setting: {ex}");
                    return false;
                }
            }
        }

        /// <summary>
        /// Initialize multiplayer compatibility detection
        /// </summary>
        public static void Initialize()
        {
            if (_initialized) return;

            try
            {
                Log.Message("[RimAsync] Initializing multiplayer compatibility...");

                // Try to find the RimWorld Multiplayer assembly
                foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
                {
                    if (assembly.GetName().Name == "Multiplayer")
                    {
                        _multiplayerAssembly = assembly;
                        break;
                    }
                }

                if (_multiplayerAssembly == null)
                {
                    Log.Message("[RimAsync] RimWorld Multiplayer not detected - single player optimizations enabled");
                    _initialized = true;
                    return;
                }

                // Try to get MultiplayerAPI type
                _multiplayerAPIType = _multiplayerAssembly.GetType("Multiplayer.Client.MultiplayerAPI");
                if (_multiplayerAPIType != null)
                {
                    _isInMultiplayerProperty = _multiplayerAPIType.GetProperty("IsInMultiplayer", BindingFlags.Public | BindingFlags.Static);
                }

                // Try to get multiplayer settings for AsyncTime
                _multiplayerSettingsType = _multiplayerAssembly.GetType("Multiplayer.Client.ClientPrefs");
                if (_multiplayerSettingsType != null)
                {
                    _asyncTimeProperty = _multiplayerSettingsType.GetProperty("AsyncTime", BindingFlags.Public | BindingFlags.Static);
                }

                _multiplayerLoaded = _isInMultiplayerProperty != null;

                if (_multiplayerLoaded)
                {
                    Log.Message("[RimAsync] RimWorld Multiplayer detected - multiplayer compatibility enabled");
                    if (_asyncTimeProperty != null)
                    {
                        Log.Message("[RimAsync] AsyncTime setting detection enabled");
                    }
                    else
                    {
                        Log.Warning("[RimAsync] AsyncTime setting not found - assuming disabled");
                    }
                }
                else
                {
                    Log.Warning("[RimAsync] RimWorld Multiplayer assembly found but API not accessible");
                }
            }
            catch (Exception ex)
            {
                Log.Error($"[RimAsync] Error initializing multiplayer compatibility: {ex}");
                _multiplayerLoaded = false;
            }
            finally
            {
                _initialized = true;
            }
        }

        /// <summary>
        /// Get a description of the current multiplayer state for debugging
        /// </summary>
        public static string GetMultiplayerStatus()
        {
            if (!_initialized) return "Not initialized";
            if (!_multiplayerLoaded) return "Multiplayer not loaded";
            
            var inMultiplayer = IsInMultiplayer;
            var asyncTime = AsyncTimeEnabled;
            
            return $"In Multiplayer: {inMultiplayer}, AsyncTime: {asyncTime}";
        }
    }
} 