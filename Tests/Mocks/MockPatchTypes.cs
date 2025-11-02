using System;
using Verse;
using Verse.AI;

namespace Tests.Mocks
{
    /// <summary>
    /// Mock Pawn for testing
    /// </summary>
    public class MockPawn : Pawn
    {
        public bool Dead { get; set; }
        public bool Spawned { get; set; }
        public Map Map { get; set; }
        public string LabelShort => "MockPawn";
        public IntVec3 Position { get; set; }
        public bool InCombat => false;

        public MockPawn()
        {
            Dead = false;
            Spawned = true;
            Position = new IntVec3(5, 0, 5);
        }
    }

    /// <summary>
    /// Mock Map for testing
    /// </summary>
    public class MockMap : Map
    {
        public MockMap() { }
    }

    /// <summary>
    /// Mock Pawn_MindState for testing
    /// </summary>
    public class MockMindState : Pawn_MindState
    {
        public Pawn pawn;

        public MockMindState()
        {
            this.pawn = new MockPawn();
        }
    }

    /// <summary>
    /// Mock Pawn_JobTracker for testing
    /// </summary>
    public class MockJobTracker : Pawn_JobTracker
    {
        public Pawn pawn;
        public Verse.AI.Job curJob;

        public MockJobTracker()
        {
            this.pawn = new MockPawn();
        }
    }

    /// <summary>
    /// Mock Thing for testing buildings
    /// </summary>
    public class MockThing : Thing
    {
        public ThingDef def { get; set; }
        public bool Spawned { get; set; }
        public Map Map { get; set; }
        public IntVec3 Position { get; set; }

        public MockThing()
        {
            Spawned = true;
            Position = IntVec3.Zero;
        }
    }

    /// <summary>
    /// Mock Pawn_PathFollower for testing
    /// </summary>
    public class MockPathFollower : Pawn_PathFollower
    {
        public Pawn pawn;

        public MockPathFollower()
        {
            this.pawn = new MockPawn();
        }
    }
}


