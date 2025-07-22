// Mock references for RimWorld.Mod classes
// Used for Docker compilation when RimWorld assemblies are not available

using System;
using Verse;

namespace RimWorld
{
    public abstract class Mod
    {
        public ModContentPack Content { get; private set; }
        public ModSettings Settings { get; private set; }
        
        public Mod(ModContentPack content)
        {
            Content = content;
        }
        
        public virtual void DoSettingsWindowContents(UnityEngine.Rect inRect) { }
        public virtual string SettingsCategory() { return null; }
        public virtual void WriteSettings() { }
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

    public abstract class GameComponent
    {
        public Game game;
        
        public GameComponent(Game game)
        {
            this.game = game;
        }
        
        public virtual void GameComponentTick() { }
        public virtual void GameComponentUpdate() { }
        public virtual void GameComponentOnGUI() { }
        public virtual void StartedNewGame() { }
        public virtual void LoadedGame() { }
        public virtual void ExposeData() { }
    }

    public static class Current
    {
        public static Game Game => Verse.Game.CurrentGame;
    }
} 