// Mock reference for RimWorld Verse.Pawn
// Used for Docker compilation when RimWorld assemblies are not available

using System.Collections.Generic;

namespace Verse
{
    public class Pawn : Thing
    {
        public Pawn_PathFollower pather;
        public Pawn_JobTracker jobs;
        public Pawn_MindState mindState;
        public PawnKindDef kindDef;
        public Name Name;

        public bool Spawned { get; set; }
        public bool Dead { get; set; }

        // Add missing properties
        public string LabelShort => Name?.ToStringShort() ?? "Unknown";
        public bool InCombat { get; set; } = false;
        public Pawn_HealthTracker health { get; set; } = new Pawn_HealthTracker();
        public Pawn_NeedsTracker needs { get; set; } = new Pawn_NeedsTracker();

        public virtual void Tick() { }
        public virtual void SpawnSetup(Map map, bool respawningAfterLoad) { }
        public virtual void DeSpawn(DestroyMode mode = DestroyMode.Vanish) { }
    }

    public class Pawn_PathFollower
    {
        public Pawn pawn;
        public bool Moving { get; set; }
        public IntVec3 Destination { get; set; }
        public PawnPath curPath;

        public virtual void PatherTick() { }
        public virtual bool TrySetNewPath() { return false; }
        public virtual void StopDead() { }
    }

    public class Pawn_JobTracker
    {
        public Pawn pawn;
        public Job curJob;
        public JobDriver curDriver;

        public virtual void JobTrackerTick() { }
        public virtual bool TryTakeOrderedJob(Job job) { return false; }
        public virtual void EndCurrentJob(JobCondition condition) { }
    }

    public class Pawn_MindState
    {
        public Pawn pawn;
        public bool Active { get; set; }

        public virtual void MindStateTick() { }
    }

    public class PawnKindDef : Def
    {
        public string defName;
        public string label;
    }

    public class Name
    {
        public string ToStringFull { get; set; }

        public string ToStringShort()
        {
            return ToStringFull?.Split(' ')[0] ?? "Unknown";
        }
    }

    public class Pawn_HealthTracker
    {
        public float summaryHealth { get; set; } = 1.0f;

        public bool HasHediffs { get; set; } = false;
        public bool InPain { get; set; } = false;
        public bool Dead { get; set; } = false;
    }

    public class Pawn_NeedsTracker
    {
        public Need_Food food { get; set; } = new Need_Food();
        public Need_Rest rest { get; set; } = new Need_Rest();
        public Need_Mood mood { get; set; } = new Need_Mood();
    }

    public abstract class Need
    {
        public float CurLevel { get; set; } = 0.5f;
        public float MaxLevel { get; set; } = 1.0f;
        public string def { get; set; } = "MockNeed";
    }

    public class Need_Food : Need
    {
        public Need_Food()
        {
            CurLevel = 0.8f;
            def = "Food";
        }
    }

    public class Need_Rest : Need
    {
        public Need_Rest()
        {
            CurLevel = 0.7f;
            def = "Rest";
        }
    }

    public class Need_Mood : Need
    {
        public Need_Mood()
        {
            CurLevel = 0.6f;
            def = "Mood";
        }
    }
}
