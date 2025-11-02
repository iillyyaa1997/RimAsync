using System;
using Verse;
using RimWorld;

namespace RimAsync.Research
{
    /// <summary>
    /// Test file to research RimWorld 1.6 API changes
    /// This file will be compiled to verify available types
    /// </summary>
    public class RimWorld16APITest
    {
        // Test 1: Check if Verse.GameComponent exists
        public void TestVerseGameComponent()
        {
            // Try to reference Verse.GameComponent
            // If compilation succeeds, it exists in Verse namespace
            Type verseGameComponent = typeof(Verse.GameComponent);
            Console.WriteLine($"Verse.GameComponent found: {verseGameComponent != null}");
        }

        // Test 2: Check if RimWorld.GameComponent exists
        public void TestRimWorldGameComponent()
        {
            // Try to reference RimWorld.GameComponent
            // If compilation fails, it doesn't exist in RimWorld namespace
            #if false
            Type rimworldGameComponent = typeof(RimWorld.GameComponent);
            Console.WriteLine($"RimWorld.GameComponent found: {rimworldGameComponent != null}");
            #endif
        }

        // Test 3: Check Building.Tick signature
        public void TestBuildingTick()
        {
            // Get Building type and find Tick method
            var buildingType = typeof(Building);
            var tickMethod = buildingType.GetMethod("Tick");

            if (tickMethod != null)
            {
                Console.WriteLine($"Building.Tick found");
                Console.WriteLine($"Return type: {tickMethod.ReturnType}");
                Console.WriteLine($"Parameters: {tickMethod.GetParameters().Length}");
                foreach (var param in tickMethod.GetParameters())
                {
                    Console.WriteLine($"  - {param.ParameterType.Name} {param.Name}");
                }
            }
            else
            {
                Console.WriteLine("Building.Tick not found");
            }
        }

        // Test 4: Check Thing.Tick signature (Building inherits from Thing)
        public void TestThingTick()
        {
            var thingType = typeof(Thing);
            var tickMethod = thingType.GetMethod("Tick");

            if (tickMethod != null)
            {
                Console.WriteLine($"Thing.Tick found");
                Console.WriteLine($"Return type: {tickMethod.ReturnType}");
                Console.WriteLine($"Parameters: {tickMethod.GetParameters().Length}");
            }
        }
    }

    // Test GameComponent in Verse namespace - RimWorld 1.6
    public class TestGameComponent : Verse.GameComponent
    {
        // RimWorld 1.6 API Change: GameComponent constructor changed
        // Old (1.5): public GameComponent(Game game)
        // New (1.6): public GameComponent() - no parameters!

        public TestGameComponent() : base()
        {
        }

        public override void GameComponentTick()
        {
            base.GameComponentTick();
        }
    }
}
