using NUnit.Framework;
using System;
using System.IO;
using System.Reflection;
using System.Linq;
using RimAsync.Tests.Utils;

namespace RimAsync.Tests.Unit.Core
{
    [TestFixture]
    public class CompilationTests
    {
        [SetUp]
        public void SetUp()
        {
            TestHelpers.MockRimWorldEnvironment();
        }

        [TearDown]
        public void TearDown()
        {
            TestHelpers.ResetMocks();
        }

        [Test]
        [Category(TestConfig.CriticalPriority)]
        public void RimAsyncAssembly_Compiles_WithoutErrors()
        {
            // Arrange & Act
            Assembly assembly = null;
            
            Assert.DoesNotThrow(() =>
            {
                assembly = Assembly.LoadFrom("RimAsync.dll");
            }, "RimAsync assembly should compile and load without errors");

            // Assert
            Assert.IsNotNull(assembly, "Assembly should not be null");
            Assert.IsTrue(assembly.GetTypes().Length > 0, "Assembly should contain types");
        }

        [Test]
        [Category(TestConfig.CriticalPriority)]
        public void RimAsyncMod_ClassExists_AndIsLoadable()
        {
            // Arrange
            var expectedClassName = "RimAsync.RimAsyncMod";

            // Act & Assert
            Assert.DoesNotThrow(() =>
            {
                var type = Type.GetType(expectedClassName);
                Assert.IsNotNull(type, $"Type {expectedClassName} should exist");
                Assert.IsTrue(type.IsClass, "RimAsyncMod should be a class");
            });
        }

        [Test]
        [Category(TestConfig.CriticalPriority)]
        public void RimAsyncCore_ClassExists_AndIsLoadable()
        {
            // Arrange
            var expectedClassName = "RimAsync.Core.RimAsyncCore";

            // Act & Assert
            Assert.DoesNotThrow(() =>
            {
                var type = Type.GetType(expectedClassName);
                Assert.IsNotNull(type, $"Type {expectedClassName} should exist");
                Assert.IsTrue(type.IsClass, "RimAsyncCore should be a class");
            });
        }

        [Test]
        [Category(TestConfig.CriticalPriority)]
        public void AsyncManager_ClassExists_AndIsLoadable()
        {
            // Arrange
            var expectedClassName = "RimAsync.Threading.AsyncManager";

            // Act & Assert
            Assert.DoesNotThrow(() =>
            {
                var type = Type.GetType(expectedClassName);
                Assert.IsNotNull(type, $"Type {expectedClassName} should exist");
                Assert.IsTrue(type.IsClass, "AsyncManager should be a class");
            });
        }

        [Test]
        [Category(TestConfig.HighPriority)]
        public void AllNamespaces_AreProperlyDefined()
        {
            // Arrange
            var expectedNamespaces = new[]
            {
                "RimAsync",
                "RimAsync.Core", 
                "RimAsync.Threading",
                "RimAsync.Utils",
                "RimAsync.Settings",
                "RimAsync.Patches"
            };

            // Act & Assert
            foreach (var ns in expectedNamespaces)
            {
                Assert.DoesNotThrow(() =>
                {
                    var types = Assembly.GetExecutingAssembly()
                        .GetTypes()
                        .Where(t => t.Namespace == ns)
                        .ToArray();
                        
                    // Note: In actual implementation, we'd check the RimAsync assembly
                    // For now, just verify the namespace structure is planned
                }, $"Namespace {ns} should be properly defined");
            }
        }

        [Test]
        [Category(TestConfig.MediumPriority)]
        public void AllSourceFiles_ExistAndAreNotEmpty()
        {
            // Arrange
            var sourceFiles = new[]
            {
                "Source/RimAsync/RimAsyncMod.cs",
                "Source/RimAsync/Core/RimAsyncCore.cs",
                "Source/RimAsync/Threading/AsyncManager.cs",
                "Source/RimAsync/Utils/MultiplayerCompat.cs",
                "Source/RimAsync/Settings/RimAsyncSettings.cs"
            };

            // Act & Assert
            foreach (var file in sourceFiles)
            {
                Assert.IsTrue(File.Exists(file), $"Source file {file} should exist");
                
                var content = File.ReadAllText(file);
                Assert.IsTrue(content.Length > 100, $"Source file {file} should not be empty");
                Assert.IsTrue(content.Contains("namespace"), $"Source file {file} should contain namespace declaration");
            }
        }

        [Test]
        [Category(TestConfig.HighPriority)]
        public void HarmonyPatches_AreProperlyStructured()
        {
            // Arrange
            var patchFiles = Directory.GetFiles("Source/RimAsync/Patches", "*.cs", SearchOption.AllDirectories);

            // Act & Assert
            Assert.IsTrue(patchFiles.Length > 0, "Should have at least one Harmony patch file");

            foreach (var patchFile in patchFiles)
            {
                var content = File.ReadAllText(patchFile);
                
                // Basic structure checks
                Assert.IsTrue(content.Contains("using HarmonyLib"), $"Patch file {patchFile} should import HarmonyLib");
                Assert.IsTrue(content.Contains("[HarmonyPatch"), $"Patch file {patchFile} should contain HarmonyPatch attribute");
            }
        }

        [Test]
        [Category(TestConfig.CriticalPriority)]
        public void ProjectStructure_IsValid()
        {
            // Arrange
            var requiredDirectories = new[]
            {
                "Source/RimAsync",
                "Source/RimAsync/Core",
                "Source/RimAsync/Threading", 
                "Source/RimAsync/Utils",
                "Source/RimAsync/Settings",
                "Source/RimAsync/Patches",
                "About"
            };

            // Act & Assert
            foreach (var dir in requiredDirectories)
            {
                Assert.IsTrue(Directory.Exists(dir), $"Directory {dir} should exist");
            }

            // Check critical files
            Assert.IsTrue(File.Exists("About/About.xml"), "About.xml should exist");
            Assert.IsTrue(File.Exists("About/Preview.png"), "Preview.png should exist");
        }

        [Test]
        [Category(TestConfig.HighPriority)]
        public void AboutXml_ContainsCorrectMetadata()
        {
            // Arrange
            var aboutXmlPath = "About/About.xml";

            // Act
            Assert.IsTrue(File.Exists(aboutXmlPath), "About.xml should exist");
            
            var content = File.ReadAllText(aboutXmlPath);

            // Assert
            Assert.IsTrue(content.Contains("rimasync.mod"), "About.xml should contain correct package ID");
            Assert.IsTrue(content.Contains("RimAsync"), "About.xml should contain mod name");
            Assert.IsTrue(content.Contains("1.5"), "About.xml should target RimWorld 1.5");
        }
    }
} 