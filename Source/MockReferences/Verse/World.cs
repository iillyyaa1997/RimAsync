// Mock references for Verse.World class
// Used for Docker compilation when RimWorld assemblies are not available

using System.Collections.Generic;

namespace Verse
{
    public class World
    {
        public WorldGrid grid;
        public WorldObjectsHolder worldObjects;
        public Game game;
        public List<Map> maps = new List<Map>();
        
        public World()
        {
            grid = new WorldGrid();
            worldObjects = new WorldObjectsHolder();
        }
        
        public virtual void WorldUpdate() { }
        public virtual void ExposeData() { }
    }

    public class WorldGrid
    {
        public int TilesCount { get; set; } = 10000;
        
        public virtual int[] GetTileIDToNeighborsOffsets(int tileID) 
        { 
            return new int[0]; 
        }
    }

    public class WorldObjectsHolder
    {
        public List<WorldObject> AllWorldObjects = new List<WorldObject>();
        
        public virtual void Add(WorldObject o) 
        { 
            AllWorldObjects.Add(o); 
        }
        
        public virtual void Remove(WorldObject o) 
        { 
            AllWorldObjects.Remove(o); 
        }
    }

    public abstract class WorldObject
    {
        public int ID;
        public int Tile;
        public World world;
        
        public virtual void SpawnSetup() { }
        public virtual void PostRemove() { }
        public virtual void ExposeData() { }
    }
} 