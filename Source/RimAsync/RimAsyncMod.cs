using System;
using HarmonyLib;
using RimWorld;
using UnityEngine;
using Verse;
using RimAsync.Utils;

namespace RimAsync
{
    /// <summary>
    /// Main mod class for RimAsync - handles initialization and lifecycle
    /// </summary>
    public class RimAsyncMod : Mod
    {
        public static RimAsyncMod Instance { get; private set; }
        public static RimAsync.Core.RimAsyncSettings Settings => Instance?.GetSettings<RimAsync.Core.RimAsyncSettings>();

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
                RimAsync.Core.RimAsyncCore.Initialize();

                // Apply patches (optionally skipped in test environment)
                var skipHarmonyEnv = Environment.GetEnvironmentVariable("RIMASYNC_SKIP_HARMONY");
                var skipHarmony = string.Equals(skipHarmonyEnv, "1", StringComparison.OrdinalIgnoreCase)
                                   || string.Equals(skipHarmonyEnv, "true", StringComparison.OrdinalIgnoreCase);

                if (skipHarmony)
                {
                    RimAsyncLogger.Info("Skipping Harmony patching due to RIMASYNC_SKIP_HARMONY environment variable", "Mod");
                }
                else
                {
                    RimAsyncLogger.Debug("Applying Harmony patches", "Mod");
                    try
                    {
                        _harmonyInstance.PatchAll();
                    }
                    catch (Exception ex)
                    {
                        // In non-RimWorld test environments, Harmony may fail. Log and continue.
                        RimAsyncLogger.Error("Harmony patching failed; continuing without patches in this environment", ex, "Mod");
                    }
                }

                RimAsyncLogger.InitStep("RimAsyncMod", "Successfully initialized with Harmony ID: rimasync.mod", true);
            }
            catch (Exception ex)
            {
                RimAsyncLogger.Error("Critical error during initialization", ex, "Mod");
                // Do not rethrow to keep tests and non-game environments resilient
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
            RimAsync.Core.RimAsyncCore.OnSettingsChanged();
        }
    }
}
