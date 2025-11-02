using System;
using HarmonyLib;
using RimWorld;
using UnityEngine;
using Verse;

namespace RimAsync
{
    /// <summary>
    /// RimWorld 1.6 compatible version - core functionality only
    /// Excluded: Building_Patch, GameComponent, Pawn AI patches (API incompatibilities)
    /// </summary>
    public class RimAsyncMod : Mod
    {
        public static RimAsyncMod Instance { get; private set; }
        public static Core.RimAsyncSettings Settings => Instance?.GetSettings<Core.RimAsyncSettings>();

        private static Harmony _harmonyInstance;
        public static Harmony HarmonyInstance => _harmonyInstance;

        public RimAsyncMod(ModContentPack content) : base(content)
        {
            Instance = this;
            Log.Message("[RimAsync] RimWorld 1.6 compatible version loaded");

            try
            {
                _harmonyInstance = new Harmony("rimasync.mod");
                Log.Message("[RimAsync] Harmony instance created");

                // Apply only compatible patches (TickManager_Patch)
                _harmonyInstance.PatchAll();
                Log.Message("[RimAsync] Compatible patches applied successfully");
            }
            catch (Exception ex)
            {
                Log.Error($"[RimAsync] Failed to apply patches: {ex}");
            }

            Log.Message("[RimAsync] Initialization complete - core async functionality enabled");
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
            Core.RimAsyncCore.OnSettingsChanged();
        }
    }
}
