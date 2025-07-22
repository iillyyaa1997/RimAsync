// Mock references for core RimWorld Verse classes
// Used for Docker compilation when RimWorld assemblies are not available

using System;
using System.Collections.Generic;

namespace Verse
{
    public class Thing
    {
        public ThingDef def;
        public IntVec3 Position;
        public Map Map;
        public bool Spawned { get; set; }
        
        public virtual void Tick() { }
        public virtual void SpawnSetup(Map map, bool respawningAfterLoad) { }
        public virtual void DeSpawn(DestroyMode mode = DestroyMode.Vanish) { }
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

    public struct IntVec3
    {
        public int x;
        public int y; 
        public int z;
        
        public IntVec3(int x, int y, int z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }
        
        public static IntVec3 Invalid => new IntVec3(-1000, -1000, -1000);
    }

    public class Map
    {
        public int Index { get; set; }
        public IntVec3 Size { get; set; }
        public Game game;
        
        public virtual void MapUpdate() { }
    }

    public class Game
    {
        public TickManager tickManager;
        public static Game CurrentGame;
        
        public virtual void GameUpdate() { }
    }

    public class TickManager
    {
        public int TicksGame { get; set; }
        public float TickRateMultiplier { get; set; } = 1.0f;
        
        public virtual void DoSingleTick() { }
    }

    public enum DestroyMode
    {
        Vanish,
        KillFinalize,
        Deconstruct,
        FailConstruction,
        Cancel,
        Refund,
        WillReplace
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
        
        public virtual IntVec3 ConsumeNextNode() { return IntVec3.Invalid; }
        public virtual void ReleaseToPool() { }
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
} 