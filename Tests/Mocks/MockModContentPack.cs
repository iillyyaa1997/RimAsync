using System;
using RimWorld;

namespace RimAsync.Tests.Mocks
{
    /// <summary>
    /// Mock implementation of ModContentPack for testing mod initialization
    /// </summary>
    public class MockModContentPack : ModContentPack
    {
        public MockModContentPack()
        {
            PackageId = "rimasync.test.mod";
            Name = "RimAsync Test Mod";
            RootDir = "/test/mod/path";
        }

        public MockModContentPack(string packageId, string name = null, string rootDir = null)
        {
            PackageId = packageId;
            Name = name ?? "Test Mod";
            RootDir = rootDir ?? "/test/mod/path";
        }
    }
}
