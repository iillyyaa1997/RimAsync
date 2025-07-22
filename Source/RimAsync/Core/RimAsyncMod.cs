using System;
using HarmonyLib;
using RimWorld;
using UnityEngine;
using Verse;

namespace RimAsync.Core
{
    /// <summary>
    /// Main mod class for RimAsync - handles initialization and lifecycle
    /// </summary>
    public class RimAsyncMod : Mod
    {
        public static RimAsyncMod Instance { get; private set; }
        public static RimAsyncSettings Settings => Instance.GetSettings<RimAsyncSettings>();
        
        private static Harmony _harmonyInstance;
        public static Harmony HarmonyInstance => _harmonyInstance;

        public RimAsyncMod(ModContentPack content) : base(content)
        {
            Instance = this;
            
            Log.Message("[RimAsync] Starting initialization...");
            
            try
            {
                // Initialize Harmony patching
                _harmonyInstance = new Harmony("rimasync.mod");
                
                // Initialize core systems
                RimAsyncCore.Initialize();
                
                // Apply patches
                _harmonyInstance.PatchAll();
                
                Log.Message("[RimAsync] Successfully initialized with Harmony ID: rimasync.mod");
            }
            catch (Exception ex)
            {
                Log.Error($"[RimAsync] Critical error during initialization: {ex}");
                throw;
            }
        }

        public override void DoSettingsWindowContents(Rect inRect)
        {
            Settings.DoWindowContents(inRect);
        }

        public override string SettingsCategory()
        {
            return "RimAsync";
        }

        public override void WriteSettings()
        {
            base.WriteSettings();
            RimAsyncCore.OnSettingsChanged();
        }
    }
} 