// Mock references for core RimWorld Verse classes
// Used for Docker compilation when RimWorld assemblies are not available

using System;
using System.Collections.Generic;
using UnityEngine;

namespace Verse
{
    // Enums and basic types
    public enum TraverseMode
    {
        ByPawn,
        NoPassClosedDoors,
        PassAllDestroyableThings,
        PassAllDestroyableThingsNotWater
    }

    public enum DestroyMode
    {
        Vanish,
        KillFinalize,
        KillFinalizeLeavingsOnly,
        Deconstruct,
        FailConstruction,
        Cancel,
        Refund,
        WillReplace
    }

    public struct IntVec3
    {
        public int x, y, z;

        public IntVec3(int x, int y, int z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }

        public static IntVec3 Zero => new IntVec3(0, 0, 0);
    }

    public struct LocalTargetInfo
    {
        public Thing thing;
        public IntVec3 cell;

        public LocalTargetInfo(Thing thing)
        {
            this.thing = thing;
            this.cell = IntVec3.Zero;
        }

        public LocalTargetInfo(IntVec3 cell)
        {
            this.thing = null;
            this.cell = cell;
        }
    }

    public class Thing
    {
        public ThingDef def;
        public bool Spawned { get; set; }
        public Map Map;
        public int index;

        public virtual void Tick() { }
        public virtual void SpawnSetup(Map map, bool respawningAfterLoad) { }
        public virtual void DeSpawn(DestroyMode mode = DestroyMode.Vanish) { }

        // Add missing properties
        public int ThingID { get; set; } = UnityEngine.Random.Range(1000, 99999);
        public IntVec3 Position { get; set; } = IntVec3.Zero;
    }

    public class ThingDef : Def
    {
        public string defName;
        public string label;
        public Type thingClass = typeof(Thing);
    }

    public abstract class Def
    {
        public string defName;
        public string label;
        public int index;
    }

    public class Map
    {
        public static Map CurrentMap { get; set; }

        public Map()
        {
            CurrentMap = this;
            pathFinder = new PathFinder();
        }

        public PathFinder pathFinder { get; private set; }
        public int Index { get; set; } = 0;
    }

    public class PathFinder
    {
        public PawnPath FindPath(IntVec3 start, LocalTargetInfo dest, Pawn pawn, TraverseMode traverseMode = TraverseMode.ByPawn, Verse.AI.PathEndMode pathEndMode = Verse.AI.PathEndMode.OnCell)
        {
            // Mock implementation - return empty path
            return new PawnPath();
        }
    }

    public class Game
    {
        public static Game CurrentGame { get; set; }
        public TickManager tickManager;
        public static Game game;

        public Game()
        {
            tickManager = new TickManager();
            CurrentGame = this;
            components = new List<RimWorld.GameComponent>();
        }

        public List<RimWorld.GameComponent> components { get; private set; }

        public T GetComponent<T>() where T : RimWorld.GameComponent
        {
            // Mock implementation - return first component of requested type
            foreach (var component in components)
            {
                if (component is T result)
                    return result;
            }
            return null;
        }

        public void AddComponent(RimWorld.GameComponent component)
        {
            components.Add(component);
        }

        public virtual void GameUpdate() { }
    }

    public class TickManager
    {
        public int TicksGame { get; set; }
        public float TickRateMultiplier { get; set; } = 1.0f;

        public virtual void DoSingleTick() { }
    }

    public class Job
    {
        public JobDef def;
        public Thing targetA;
        public IntVec3 targetB;
        public int count = -1;

        public Job() { }
        public Job(JobDef def) { this.def = def; }
        public Job(JobDef def, Thing targetA) : this(def) { this.targetA = targetA; }
    }

    public class JobDef : Def
    {
        public string defName;
        public Type driverClass;
    }

    public abstract class JobDriver
    {
        public Job job;
        public Pawn pawn;

        public virtual bool TryMakePreToilReservations(bool errorOnFailed) { return true; }
        public virtual void Notify_Starting() { }
        public virtual void DriverTick() { }
    }

    public enum JobCondition
    {
        None,
        Ongoing,
        Succeeded,
        Incompletable,
        InterruptForced,
        InterruptOptional,
        QueuedNoLongerValid,
        Errored
    }

    public class PawnPath
    {
        public bool Found { get; set; }
        public int NodesLeftCount { get; set; }
        public List<IntVec3> nodes = new List<IntVec3>();

        public virtual IntVec3 ConsumeNextNode() { return IntVec3.Zero; }
        public virtual void ReleaseToPool() { }

        public float TotalCost { get; set; } = 0f;
    }

    public static class Log
    {
        public static void Message(string text) { Console.WriteLine($"[LOG] {text}"); }
        public static void Warning(string text) { Console.WriteLine($"[WARN] {text}"); }
        public static void Error(string text) { Console.WriteLine($"[ERROR] {text}"); }
    }

    public static class Find
    {
        public static TickManager TickManager => Game.CurrentGame?.tickManager;
        public static Game Game => Game.CurrentGame;
    }

    public static class Scribe_Values
    {
        public static void Look<T>(ref T value, string label, T defaultValue = default(T), bool forceSave = false)
        {
            // Mock implementation for testing
            if (EqualityComparer<T>.Default.Equals(value, default(T)))
            {
                value = defaultValue;
            }
        }
    }

    public class Listing_Standard
    {
        private Rect _rect;
        private float _currentY;

        public void Begin(Rect rect)
        {
            _rect = rect;
            _currentY = rect.y;
        }

        public void End()
        {
            // Mock implementation
        }

        public bool CheckboxLabeled(string label, ref bool checkOn, string tooltip = null)
        {
            // Mock implementation - return if checkbox was clicked
            var wasOn = checkOn;
            // In real implementation, this would render UI and handle input
            return wasOn != checkOn;
        }

        public void Label(string label, float height = -1f)
        {
            // Mock implementation
            _currentY += height > 0 ? height : 24f;
        }

        public void Gap(float height = 12f)
        {
            _currentY += height;
        }

        public bool ButtonText(string label, float width = -1f, float height = -1f)
        {
            // Mock implementation - return if button was clicked
            _currentY += height > 0 ? height : 32f;
            return false;
        }

        public void IntEntry(ref int value, ref string buffer, int multiplier = 1)
        {
            // Mock implementation
            _currentY += 24f;
        }

        public float Slider(float value, float min, float max)
        {
            // Mock implementation - return unchanged value
            _currentY += 24f;
            return value;
        }
    }

    public static class ModsConfig
    {
        public static bool IsActive(string packageId)
        {
            // Mock implementation - assume all mods are active for testing
            return true;
        }

        public static bool RoyaltyActive => true;
        public static bool IdeologyActive => true;
        public static bool BiotechActive => true;
        public static bool AnomalyActive => true;

        public static System.Collections.Generic.List<RimWorld.ModContentPack> ActiveModsInLoadOrder
        {
            get
            {
                // Mock implementation - return empty list
                return new System.Collections.Generic.List<RimWorld.ModContentPack>();
            }
        }
    }
}
