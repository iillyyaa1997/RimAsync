using HarmonyLib;
using RimAsync.Utils;
using System;
using Verse;

namespace RimAsync.Core
{
    /// <summary>
    /// DISABLED: StaticConstructorOnStartup runs DURING Def PostLoad, not after!
    /// This was causing texture loading failures because patches interfered with
    /// TerrainDef.PostLoad and other critical initialization.
    ///
    /// Patches are now applied from RimAsyncMod constructor instead, which runs
    /// earlier and doesn't interfere with Def PostLoad phase.
    /// </summary>
    // [StaticConstructorOnStartup] // DISABLED - was breaking Def loading!
    public static class RimAsyncInitializer
    {
        // This class is kept for backwards compatibility but does nothing now
        // All initialization moved to RimAsyncMod constructor

        static RimAsyncInitializer()
        {
            // Empty - initialization moved to RimAsyncMod constructor
            RimAsyncLogger.Debug("RimAsyncInitializer (legacy) called - doing nothing", "Init");
        }
    }
}
