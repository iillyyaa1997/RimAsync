// Mock references for RimWorld.Mod classes
// Used for Docker compilation when RimWorld assemblies are not available

using System;
using Verse;

namespace RimWorld
{
    public abstract class Mod
    {
        public ModContentPack Content { get; protected set; }
        public ModSettings Settings { get; private set; }

        public Mod(ModContentPack content)
        {
            Content = content;
        }

        public virtual void DoSettingsWindowContents(UnityEngine.Rect inRect) { }
        public virtual string SettingsCategory() { return null; }
        public virtual void WriteSettings() { }

        public T GetSettings<T>() where T : ModSettings, new()
        {
            // Mock implementation - return new instance for testing
            return new T();
        }
    }

    public class ModContentPack
    {
        public string PackageId { get; set; }
        public string Name { get; set; }
        public string RootDir { get; set; }
    }

    public abstract class ModSettings
    {
        public virtual void ExposeData() { }
    }

    public class GameComponent
    {
        public Verse.Game game;

        public GameComponent()
        {
        }

        public GameComponent(Verse.Game game)
        {
            this.game = game;
        }

        public virtual void GameComponentUpdate() { }
        public virtual void GameComponentTick() { }
        public virtual void GameComponentOnGUI() { }
        public virtual void FinalizeInit() { }
    }

    public class LoadedModManager
    {
        public static ModContentPack GetModContentPack(string packageId)
        {
            return new ModContentPack { PackageId = packageId, Name = "Mock Mod" };
        }
    }

    public static class Current
    {
        public static Game Game => Verse.Game.CurrentGame;
    }
}
