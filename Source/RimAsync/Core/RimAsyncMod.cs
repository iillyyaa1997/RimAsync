using System;
using HarmonyLib;
using RimWorld;
using UnityEngine;
using Verse;
using RimAsync.Utils;

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

            // Configure logging system first
            RimAsyncLogger.Configure(enableDebug: false, RimAsyncLogger.LogLevel.Info);
            RimAsyncLogger.InitStep("RimAsyncMod", "Starting mod initialization");

            try
            {
                // Initialize Harmony patching
                RimAsyncLogger.Debug("Creating Harmony instance", "Mod");
                _harmonyInstance = new Harmony("rimasync.mod");

                // Initialize core systems
                RimAsyncLogger.Debug("Initializing core systems", "Mod");
                RimAsyncCore.Initialize();

                // Apply patches
                RimAsyncLogger.Debug("Applying Harmony patches", "Mod");
                _harmonyInstance.PatchAll();

                RimAsyncLogger.InitStep("RimAsyncMod", "Successfully initialized with Harmony ID: rimasync.mod", true);
            }
            catch (Exception ex)
            {
                RimAsyncLogger.Error("Critical error during initialization", ex, "Mod");
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
