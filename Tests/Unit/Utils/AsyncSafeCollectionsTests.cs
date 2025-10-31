using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using NUnit.Framework;
using RimAsync.Utils;
using RimAsync.Tests.Utils;
using static RimAsync.Utils.AsyncSafeCollections;

namespace RimAsync.Tests.Unit.Utils
{
    /// <summary>
    /// Unit tests for AsyncSafeCollections
    /// Tests thread-safe collection implementations
    /// </summary>
    [TestFixture]
    [Category(TestConfig.UnitTestCategory)]
    [Category(TestConfig.HighPriority)]
    public class AsyncSafeCollectionsTests
    {
        #region PriorityQueue Tests

        [Test]
        [Timeout(TestConfig.DefaultTimeoutMs)]
        public void PriorityQueue_Enqueue_DequeuesInPriorityOrder()
        {
            // Arrange
            var queue = new PriorityQueue<string>();
            
            // Act - Add items with different priorities
            queue.Enqueue("Low", 10);
            queue.Enqueue("High", 1);
            queue.Enqueue("Medium", 5);
            
            // Assert - Should dequeue in priority order (lowest first)
            Assert.That(queue.TryDequeue(out var item1) && item1 == "High", Is.True);
            Assert.That(queue.TryDequeue(out var item2) && item2 == "Medium", Is.True);
            Assert.That(queue.TryDequeue(out var item3) && item3 == "Low", Is.True);
        }

        [Test]
        [Timeout(TestConfig.DefaultTimeoutMs)]
        public void PriorityQueue_TryPeek_DoesNotRemoveItem()
        {
            // Arrange
            var queue = new PriorityQueue<int>();
            queue.Enqueue(42, 1);
            
            // Act
            var peekResult = queue.TryPeek(out var item, out var priority);
            
            // Assert
            Assert.That(peekResult, Is.True);
            Assert.That(item, Is.EqualTo(42));
            Assert.That(priority, Is.EqualTo(1));
            Assert.That(queue.Count, Is.EqualTo(1), "Peek should not remove item");
        }

        [Test]
        [Timeout(TestConfig.DefaultTimeoutMs)]
        public void PriorityQueue_Clear_RemovesAllItems()
        {
            // Arrange
            var queue = new PriorityQueue<string>();
            queue.Enqueue("A", 1);
            queue.Enqueue("B", 2);
            
            // Act
            queue.Clear();
            
            // Assert
            Assert.That(queue.Count, Is.EqualTo(0));
            Assert.That(queue.IsEmpty, Is.True);
        }

        [Test]
        [Timeout(TestConfig.PerformanceTimeoutMs)]
        public async Task PriorityQueue_ThreadSafety_HandlesConcurrentAccess()
        {
            // Arrange
            var queue = new PriorityQueue<int>();
            
            // Act
            await TestHelpers.AssertThreadSafety(async () =>
            {
                var random = new Random();
                queue.Enqueue(random.Next(100), random.Next(10));
                await Task.Delay(1);
                queue.TryDequeue(out _);
            }, concurrency: 10);
            
            // Assert - If no exceptions, thread safety is good
            Assert.Pass("PriorityQueue handled concurrent access");
        }

        #endregion

        #region VersionedDictionary Tests

        [Test]
        [Timeout(TestConfig.DefaultTimeoutMs)]
        public void VersionedDictionary_TryAdd_IncrementsVersion()
        {
            // Arrange
            var dict = new VersionedDictionary<string, int>();
            
            // Act
            dict.TryAdd("key1", 100);
            dict.TryAdd("key2", 200);
            
            // Assert
            Assert.That(dict.CurrentVersion, Is.EqualTo(2), "Version should increment");
        }

        [Test]
        [Timeout(TestConfig.DefaultTimeoutMs)]
        public void VersionedDictionary_TryGetValue_ReturnsVersion()
        {
            // Arrange
            var dict = new VersionedDictionary<string, int>();
            dict.TryAdd("test", 42);
            
            // Act
            var result = dict.TryGetValue("test", out var value, out var version);
            
            // Assert
            Assert.That(result, Is.True);
            Assert.That(value, Is.EqualTo(42));
            Assert.That(version, Is.EqualTo(1), "First add should be version 1");
        }

        [Test]
        [Timeout(TestConfig.DefaultTimeoutMs)]
        public void VersionedDictionary_TryUpdate_IncrementsVersion()
        {
            // Arrange
            var dict = new VersionedDictionary<string, int>();
            dict.TryAdd("key", 10);
            
            // Act
            dict.TryUpdate("key", 20);
            dict.TryGetValue("key", out var value, out var version);
            
            // Assert
            Assert.That(value, Is.EqualTo(20));
            Assert.That(version, Is.EqualTo(2), "Update should increment version");
        }

        [Test]
        [Timeout(TestConfig.DefaultTimeoutMs)]
        public void VersionedDictionary_GetAll_ReturnsAllEntriesWithVersions()
        {
            // Arrange
            var dict = new VersionedDictionary<string, int>();
            dict.TryAdd("a", 1);
            dict.TryAdd("b", 2);
            dict.TryAdd("c", 3);
            
            // Act
            var all = dict.GetAll().ToList();
            
            // Assert
            Assert.That(all.Count, Is.EqualTo(3));
            Assert.That(all.All(e => e.Version > 0), Is.True, "All entries should have versions");
        }

        [Test]
        [Timeout(TestConfig.DefaultTimeoutMs)]
        public void VersionedDictionary_Clear_ResetsVersion()
        {
            // Arrange
            var dict = new VersionedDictionary<string, int>();
            dict.TryAdd("key1", 1);
            dict.TryAdd("key2", 2);
            
            // Act
            dict.Clear();
            
            // Assert
            Assert.That(dict.Count, Is.EqualTo(0));
            Assert.That(dict.CurrentVersion, Is.EqualTo(0), "Version should reset");
        }

        [Test]
        [Timeout(TestConfig.PerformanceTimeoutMs)]
        public async Task VersionedDictionary_ThreadSafety_HandlesConflicts()
        {
            // Arrange
            var dict = new VersionedDictionary<int, string>();
            
            // Act - Multiple threads updating same keys
            await TestHelpers.AssertThreadSafety(async () =>
            {
                var random = new Random();
                var key = random.Next(5); // Limited key space for conflicts
                dict.TryAdd(key, "value");
                await Task.Delay(1);
                dict.TryUpdate(key, "updated");
            }, concurrency: 20);
            
            // Assert
            Assert.That(dict.CurrentVersion, Is.GreaterThan(0));
            Assert.Pass("VersionedDictionary handled concurrent updates");
        }

        #endregion

        #region ConcurrentSet Tests

        [Test]
        [Timeout(TestConfig.DefaultTimeoutMs)]
        public void ConcurrentSet_Add_PreventsДубликаты()
        {
            // Arrange
            var set = new ConcurrentSet<int>();
            
            // Act
            var firstAdd = set.Add(42);
            var secondAdd = set.Add(42);
            
            // Assert
            Assert.That(firstAdd, Is.True, "First add should succeed");
            Assert.That(secondAdd, Is.False, "Second add should fail (duplicate)");
            Assert.That(set.Count, Is.EqualTo(1));
        }

        [Test]
        [Timeout(TestConfig.DefaultTimeoutMs)]
        public void ConcurrentSet_AddRange_AddsMultipleItems()
        {
            // Arrange
            var set = new ConcurrentSet<string>();
            var items = new[] { "a", "b", "c", "a" }; // "a" is duplicate
            
            // Act
            set.AddRange(items);
            
            // Assert
            Assert.That(set.Count, Is.EqualTo(3), "Should have 3 unique items");
        }

        [Test]
        [Timeout(TestConfig.DefaultTimeoutMs)]
        public void ConcurrentSet_RemoveWhere_RemovesMatchingItems()
        {
            // Arrange
            var set = new ConcurrentSet<int>();
            for (int i = 1; i <= 10; i++)
            {
                set.Add(i);
            }
            
            // Act - Remove even numbers
            var removed = set.RemoveWhere(x => x % 2 == 0);
            
            // Assert
            Assert.That(removed, Is.EqualTo(5), "Should remove 5 even numbers");
            Assert.That(set.Count, Is.EqualTo(5), "Should have 5 odd numbers left");
        }

        [Test]
        [Timeout(TestConfig.DefaultTimeoutMs)]
        public void ConcurrentSet_IsSubsetOf_ChecksCorrectly()
        {
            // Arrange
            var set = new ConcurrentSet<int>();
            set.AddRange(new[] { 1, 2, 3 });
            
            // Act & Assert
            Assert.That(set.IsSubsetOf(new[] { 1, 2, 3, 4, 5 }), Is.True);
            Assert.That(set.IsSubsetOf(new[] { 1, 2 }), Is.False);
        }

        [Test]
        [Timeout(TestConfig.DefaultTimeoutMs)]
        public void ConcurrentSet_IsSupersetOf_ChecksCorrectly()
        {
            // Arrange
            var set = new ConcurrentSet<int>();
            set.AddRange(new[] { 1, 2, 3, 4, 5 });
            
            // Act & Assert
            Assert.That(set.IsSupersetOf(new[] { 1, 2, 3 }), Is.True);
            Assert.That(set.IsSupersetOf(new[] { 1, 6 }), Is.False);
        }

        [Test]
        [Timeout(TestConfig.DefaultTimeoutMs)]
        public void ConcurrentSet_ToHashSet_ConvertsCorrectly()
        {
            // Arrange
            var set = new ConcurrentSet<string>();
            set.AddRange(new[] { "a", "b", "c" });
            
            // Act
            var hashSet = set.ToHashSet();
            
            // Assert
            Assert.That(hashSet.Count, Is.EqualTo(3));
            Assert.That(hashSet.Contains("a"), Is.True);
        }

        [Test]
        [Timeout(TestConfig.PerformanceTimeoutMs)]
        public async Task ConcurrentSet_ThreadSafety_HandlesConcurrentOps()
        {
            // Arrange
            var set = new ConcurrentSet<int>();
            
            // Act
            await TestHelpers.AssertThreadSafety(async () =>
            {
                var random = new Random();
                var value = random.Next(100);
                set.Add(value);
                await Task.Delay(1);
                set.Contains(value);
                set.Remove(value);
            }, concurrency: 20);
            
            // Assert
            Assert.Pass("ConcurrentSet handled concurrent operations");
        }

        #endregion

        #region Existing Collections Tests

        [Test]
        [Timeout(TestConfig.DefaultTimeoutMs)]
        public void SafeList_AddAndTake_WorksCorrectly()
        {
            // Arrange
            var list = new SafeList<int>();
            
            // Act
            list.Add(1);
            list.Add(2);
            list.Add(3);
            
            // Assert
            Assert.That(list.Count, Is.EqualTo(3));
            Assert.That(list.TryTake(out var item) && item == 1, Is.True);
        }

        [Test]
        [Timeout(TestConfig.DefaultTimeoutMs)]
        public void BoundedSafeQueue_RespectsSizeLimit()
        {
            // Arrange
            var queue = new BoundedSafeQueue<int>(3);
            
            // Act
            queue.TryEnqueue(1);
            queue.TryEnqueue(2);
            queue.TryEnqueue(3);
            var fourthEnqueue = queue.TryEnqueue(4); // Should fail
            
            // Assert
            Assert.That(fourthEnqueue, Is.False, "Should not exceed max size");
            Assert.That(queue.IsFull, Is.True);
        }

        [Test]
        [Timeout(TestConfig.DefaultTimeoutMs)]
        public void SafeCounter_ThreadSafeIncrement()
        {
            // Arrange
            var counter = new SafeCounter();
            
            // Act
            var value1 = counter.Increment();
            var value2 = counter.Increment();
            
            // Assert
            Assert.That(value1, Is.EqualTo(1));
            Assert.That(value2, Is.EqualTo(2));
            Assert.That(counter.Value, Is.EqualTo(2));
        }

        [Test]
        [Timeout(TestConfig.DefaultTimeoutMs)]
        public void SafeFlag_SetAndReset_WorksCorrectly()
        {
            // Arrange
            var flag = new SafeFlag();
            
            // Act & Assert
            Assert.That(flag.IsSet, Is.False, "Initially not set");
            Assert.That(flag.Set(), Is.True, "First set should succeed");
            Assert.That(flag.IsSet, Is.True, "Should be set");
            Assert.That(flag.Set(), Is.False, "Second set should fail");
            Assert.That(flag.Reset(), Is.True, "Reset should succeed");
            Assert.That(flag.IsSet, Is.False, "Should be reset");
        }

        #endregion

        #region Performance Tests

        [Test]
        [Timeout(TestConfig.PerformanceTimeoutMs)]
        public void PriorityQueue_Performance_FastOperations()
        {
            // Arrange
            var queue = new PriorityQueue<int>();
            const int iterations = 1000;
            
            // Act
            var metrics = TestHelpers.MeasurePerformance(() =>
            {
                queue.Enqueue(42, 1);
                queue.TryDequeue(out _);
            }, iterations);
            
            // Assert
            Assert.That(metrics.AverageTimePerIteration, Is.LessThan(0.5), 
                "Priority queue ops should be fast (< 0.5ms)");
            
            TestContext.WriteLine($"PriorityQueue performance: {metrics.AverageTimePerIteration:F4}ms per op");
        }

        [Test]
        [Timeout(TestConfig.PerformanceTimeoutMs)]
        public void VersionedDictionary_Performance_LowOverhead()
        {
            // Arrange
            var dict = new VersionedDictionary<int, string>();
            const int iterations = 1000;
            
            // Act
            var metrics = TestHelpers.MeasurePerformance(() =>
            {
                dict.TryAdd(42, "value");
                dict.TryGetValue(42, out _, out _);
                dict.TryRemove(42, out _);
            }, iterations);
            
            // Assert
            Assert.That(metrics.AverageTimePerIteration, Is.LessThan(0.5), 
                "Versioned dictionary should have low overhead");
            
            TestContext.WriteLine($"VersionedDictionary performance: {metrics.AverageTimePerIteration:F4}ms per op");
        }

        [Test]
        [Timeout(TestConfig.PerformanceTimeoutMs)]
        public void ConcurrentSet_Performance_BulkOperations()
        {
            // Arrange
            var set = new ConcurrentSet<int>();
            const int bulkSize = 100;
            var items = Enumerable.Range(0, bulkSize).ToArray();
            
            // Act
            var metrics = TestHelpers.MeasurePerformance(() =>
            {
                set.AddRange(items);
                set.RemoveWhere(x => x % 2 == 0);
            }, iterations: 10);
            
            // Assert
            Assert.That(metrics.AverageTimePerIteration, Is.LessThan(10), 
                "Bulk operations should be reasonably fast");
            
            TestContext.WriteLine($"ConcurrentSet bulk ops: {metrics.AverageTimePerIteration:F2}ms per {bulkSize} items");
        }

        #endregion
    }
}

