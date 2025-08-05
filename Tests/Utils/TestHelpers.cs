using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Collections.Generic;
using NUnit.Framework;
using RimAsync.Core;
using RimAsync.Utils;
using Verse;

namespace RimAsync.Tests.Utils
{
    /// <summary>
    /// Helper utilities for RimAsync testing
    /// </summary>
    public static class TestHelpers
    {
        private static readonly object _mockLock = new object();
        private static bool _rimWorldMocked = false;
        private static bool _multiplayerMocked = false;

        /// <summary>
        /// Mock RimWorld environment for testing
        /// </summary>
        public static void MockRimWorldEnvironment()
        {
            lock (_mockLock)
            {
                if (_rimWorldMocked) return;

                // Mock basic RimWorld game state
                // This would be implemented with actual RimWorld mocking
                _rimWorldMocked = true;
            }
        }

        /// <summary>
        /// Mock multiplayer state with AsyncTime setting
        /// </summary>
        public static void MockMultiplayerState(bool asyncTimeEnabled)
        {
            lock (_mockLock)
            {
                if (_multiplayerMocked) return;

                // Mock multiplayer mod detection and AsyncTime state
                // This would integrate with actual multiplayer mod mocking
                _multiplayerMocked = true;
            }
        }

        /// <summary>
        /// Reset all mocks to clean state
        /// </summary>
        public static void ResetMocks()
        {
            lock (_mockLock)
            {
                _rimWorldMocked = false;
                _multiplayerMocked = false;
            }
        }

        /// <summary>
        /// Validate no new desync files were created during test
        /// </summary>
        public static bool ValidateNoDesyncs()
        {
            if (!Directory.Exists(TestConfig.MpDesyncPath))
                return true; // No desyncs folder means no desyncs

            var files = Directory.GetFiles(TestConfig.MpDesyncPath, "*", SearchOption.TopDirectoryOnly);

            // In real implementation, would check file timestamps against test start time
            return files.Length == 0;
        }

        /// <summary>
        /// Measure performance of an action with detailed metrics
        /// </summary>
        public static PerformanceMetrics MeasurePerformance(Action action, int iterations = 1)
        {
            var metrics = new PerformanceMetrics();
            var stopwatch = new Stopwatch();

            // Warm up
            action();

            GC.Collect();
            GC.WaitForPendingFinalizers();
            GC.Collect();

            var startMemory = GC.GetTotalMemory(false);

            stopwatch.Start();
            for (int i = 0; i < iterations; i++)
            {
                action();
            }
            stopwatch.Stop();

            var endMemory = GC.GetTotalMemory(false);

            metrics.ElapsedMilliseconds = stopwatch.ElapsedMilliseconds;
            metrics.MemoryUsed = endMemory - startMemory;
            metrics.Iterations = iterations;
            metrics.AverageTimePerIteration = (double)stopwatch.ElapsedMilliseconds / iterations;

            return metrics;
        }

        /// <summary>
        /// Measure async performance with cancellation support
        /// </summary>
        public static async Task<PerformanceMetrics> MeasurePerformanceAsync(
            Func<CancellationToken, Task> action,
            int iterations = 1,
            CancellationToken cancellationToken = default)
        {
            var metrics = new PerformanceMetrics();
            var stopwatch = new Stopwatch();

            // Warm up
            await action(cancellationToken);

            GC.Collect();
            GC.WaitForPendingFinalizers();
            GC.Collect();

            var startMemory = GC.GetTotalMemory(false);

            stopwatch.Start();
            for (int i = 0; i < iterations; i++)
            {
                await action(cancellationToken);
            }
            stopwatch.Stop();

            var endMemory = GC.GetTotalMemory(false);

            metrics.ElapsedMilliseconds = stopwatch.ElapsedMilliseconds;
            metrics.MemoryUsed = endMemory - startMemory;
            metrics.Iterations = iterations;
            metrics.AverageTimePerIteration = (double)stopwatch.ElapsedMilliseconds / iterations;

            return metrics;
        }

        /// <summary>
        /// Create temporary test file
        /// </summary>
        public static string CreateTempTestFile(string content = "")
        {
            var tempPath = Path.Combine(TestConfig.TestDataPath, $"test_{Guid.NewGuid()}.tmp");
            Directory.CreateDirectory(Path.GetDirectoryName(tempPath));
            File.WriteAllText(tempPath, content);
            return tempPath;
        }

        /// <summary>
        /// Clean up temporary test files
        /// </summary>
        public static void CleanupTempFiles()
        {
            if (Directory.Exists(TestConfig.TestDataPath))
            {
                foreach (var file in Directory.GetFiles(TestConfig.TestDataPath, "test_*.tmp"))
                {
                    try
                    {
                        File.Delete(file);
                    }
                    catch
                    {
                        // Ignore cleanup errors
                    }
                }
            }
        }

        /// <summary>
        /// Assert that performance improvement meets target
        /// </summary>
        public static void AssertPerformanceImprovement(
            double baselineMs,
            double optimizedMs,
            float expectedImprovement,
            string context = "")
        {
            var actualImprovement = (baselineMs - optimizedMs) / baselineMs;

            if (actualImprovement < expectedImprovement)
            {
                throw new AssertionException(
                    $"Performance improvement {actualImprovement:P2} is below target {expectedImprovement:P2}" +
                    (string.IsNullOrEmpty(context) ? "" : $" for {context}"));
            }
        }

        /// <summary>
        /// Assert thread safety by running action concurrently
        /// </summary>
        public static async Task AssertThreadSafety(Func<Task> action, int concurrency = 10)
        {
            var tasks = new List<Task>();
            var exceptions = new List<Exception>();

            for (int i = 0; i < concurrency; i++)
            {
                tasks.Add(Task.Run(async () =>
                {
                    try
                    {
                        await action();
                    }
                    catch (Exception ex)
                    {
                        lock (exceptions)
                        {
                            exceptions.Add(ex);
                        }
                    }
                }));
            }

            await Task.WhenAll(tasks);

            if (exceptions.Count > 0)
            {
                throw new AggregateException("Thread safety test failed", exceptions);
            }
        }

        /// <summary>
        /// Create a cancellation token that times out after specified milliseconds
        /// </summary>
        public static CancellationToken CreateTimeoutToken(int timeoutMs = 5000)
        {
            return new CancellationTokenSource(timeoutMs).Token;
        }

        /// <summary>
        /// Create a mock Pawn for testing
        /// </summary>
        public static Verse.Pawn CreateMockPawn()
        {
            var pawn = new Verse.Pawn
            {
                Spawned = true,
                Dead = false,
                InCombat = false,
                Map = CreateMockMap()
            };

            // Initialize required components
            pawn.pather = new Verse.Pawn_PathFollower { pawn = pawn };
            pawn.jobs = new Verse.Pawn_JobTracker { pawn = pawn };
            pawn.mindState = new Verse.Pawn_MindState { pawn = pawn, Active = true };
            pawn.kindDef = new Verse.PawnKindDef { defName = "TestPawn", label = "Test Pawn" };
            pawn.Name = new Verse.Name { ToStringFull = "Test Pawn" };

            // Initialize health and needs
            pawn.health = new Verse.Pawn_HealthTracker { summaryHealth = 1.0f };
            pawn.needs = new Verse.Pawn_NeedsTracker();

            return pawn;
        }

        /// <summary>
        /// Create a mock Building for testing
        /// </summary>
        public static Thing CreateMockBuilding()
        {
            var building = new Thing
            {
                def = new ThingDef { defName = "Wall", label = "Wall" },
                Spawned = true,
                Map = CreateMockMap()
            };

            return building;
        }

        /// <summary>
        /// Create a mock Construction for testing
        /// </summary>
        public static Thing CreateMockConstruction()
        {
            var construction = new Thing
            {
                def = new ThingDef { defName = "Frame_Wall", label = "Wall Frame" },
                Spawned = true,
                Map = CreateMockMap()
            };

            return construction;
        }

        /// <summary>
        /// Create a mock Thing with specific defName for testing
        /// </summary>
        public static Thing CreateMockThing(string defName)
        {
            var thing = new Thing
            {
                def = new ThingDef { defName = defName, label = defName },
                Spawned = true,
                Map = CreateMockMap()
            };

            return thing;
        }

        /// <summary>
        /// Create a mock Map for testing
        /// </summary>
        public static Verse.Map CreateMockMap()
        {
            var map = new Verse.Map
            {
                Index = 0
            };

            return map;
        }
    }

    /// <summary>
    /// Performance measurement results
    /// </summary>
    public class PerformanceMetrics
    {
        public long ElapsedMilliseconds { get; set; }
        public long MemoryUsed { get; set; }
        public int Iterations { get; set; }
        public double AverageTimePerIteration { get; set; }

        public override string ToString()
        {
            return $"Elapsed: {ElapsedMilliseconds}ms, Memory: {MemoryUsed} bytes, " +
                   $"Avg per iteration: {AverageTimePerIteration:F2}ms";
        }
    }

    /// <summary>
    /// Custom assertion exception for test failures
    /// </summary>
    public class AssertionException : Exception
    {
        public AssertionException(string message) : base(message) { }
        public AssertionException(string message, Exception innerException) : base(message, innerException) { }
    }
}
