// Mock references for Verse.AI classes
// Used for Docker compilation when RimWorld assemblies are not available

namespace Verse.AI
{
    public enum PathEndMode
    {
        OnCell,
        Touch,
        ClosestTouch,
        InteractionCell
    }

    public struct ThinkResult
    {
        public Job Job { get; set; }
        public JobDef SourceJobDef { get; set; }
        
        public ThinkResult(Job job)
        {
            Job = job;
            SourceJobDef = job?.def;
        }
        
        public static ThinkResult NoJob => new ThinkResult();
    }

    public abstract class JobGiver
    {
        public virtual ThinkResult TryGiveJob(Pawn pawn) { return ThinkResult.NoJob; }
    }

    public abstract class ThinkNode
    {
        public virtual ThinkResult TryIssueJobPackage(Pawn pawn, JobIssueParams jobParams) 
        { 
            return ThinkResult.NoJob; 
        }
    }

    public struct JobIssueParams
    {
        public bool canTakeInventory;
        public bool canBash;
        public bool canDig;
        
        public JobIssueParams(bool canTakeInventory = true, bool canBash = false, bool canDig = false)
        {
            this.canTakeInventory = canTakeInventory;
            this.canBash = canBash;
            this.canDig = canDig;
        }
    }
} 