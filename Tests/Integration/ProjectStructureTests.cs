using NUnit.Framework;
using System;
using System.IO;
using System.Linq;
using RimAsync.Tests.Utils;

namespace RimAsync.Tests.Integration
{
    [TestFixture]
    public class ProjectStructureTests
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
            TestHelpers.CleanupTempFiles();
        }

        [Test]
        [Category(TestConfig.CriticalPriority)]
        public void SourceDirectory_ExistsAndContainsRimAsyncFolder()
        {
            // Arrange & Act & Assert
            Assert.IsTrue(Directory.Exists("Source"), "Source directory should exist");
            Assert.IsTrue(Directory.Exists("Source/RimAsync"), "Source/RimAsync directory should exist");
            
            var sourceFiles = Directory.GetFiles("Source/RimAsync", "*.cs", SearchOption.AllDirectories);
            Assert.IsTrue(sourceFiles.Length > 0, "Source directory should contain C# files");
        }

        [Test]
        [Category(TestConfig.CriticalPriority)]
        public void AboutDirectory_ContainsRequiredFiles()
        {
            // Arrange
            var requiredFiles = new[]
            {
                "About/About.xml",
                "About/Preview.png"
            };

            // Act & Assert
            Assert.IsTrue(Directory.Exists("About"), "About directory should exist");

            foreach (var file in requiredFiles)
            {
                Assert.IsTrue(File.Exists(file), $"Required file {file} should exist");
                
                var fileInfo = new FileInfo(file);
                Assert.IsTrue(fileInfo.Length > 0, $"File {file} should not be empty");
            }
        }

        [Test]
        [Category(TestConfig.HighPriority)]
        public void CoreComponents_AreProperlyStructured()
        {
            // Arrange
            var coreFiles = new[]
            {
                "Source/RimAsync/RimAsyncMod.cs",
                "Source/RimAsync/Core/RimAsyncCore.cs",
                "Source/RimAsync/Threading/AsyncManager.cs",
                "Source/RimAsync/Utils/MultiplayerCompat.cs",
                "Source/RimAsync/Settings/RimAsyncSettings.cs"
            };

            // Act & Assert
            foreach (var file in coreFiles)
            {
                Assert.IsTrue(File.Exists(file), $"Core file {file} should exist");
                
                var content = File.ReadAllText(file);
                
                // Check basic C# structure
                Assert.IsTrue(content.Contains("using"), $"File {file} should contain using statements");
                Assert.IsTrue(content.Contains("namespace"), $"File {file} should contain namespace declaration");
                Assert.IsTrue(content.Contains("class") || content.Contains("interface"), 
                    $"File {file} should contain class or interface declaration");
            }
        }

        [Test]
        [Category(TestConfig.HighPriority)]
        public void PatchesDirectory_ContainsValidHarmonyPatches()
        {
            // Arrange
            var patchesDir = "Source/RimAsync/Patches";

            // Act & Assert
            Assert.IsTrue(Directory.Exists(patchesDir), "Patches directory should exist");
            
            var patchFiles = Directory.GetFiles(patchesDir, "*Patch.cs", SearchOption.AllDirectories);
            Assert.IsTrue(patchFiles.Length > 0, "Should contain at least one patch file");

            foreach (var patchFile in patchFiles)
            {
                var content = File.ReadAllText(patchFile);
                
                // Validate Harmony patch structure
                Assert.IsTrue(content.Contains("using HarmonyLib"), 
                    $"Patch {patchFile} should import HarmonyLib");
                Assert.IsTrue(content.Contains("[HarmonyPatch"), 
                    $"Patch {patchFile} should contain HarmonyPatch attribute");
                Assert.IsTrue(content.Contains("static") && (content.Contains("Prefix") || content.Contains("Postfix")), 
                    $"Patch {patchFile} should contain static Prefix or Postfix method");
            }
        }

        [Test]
        [Category(TestConfig.MediumPriority)]
        public void TestingInfrastructure_IsProperlySetUp()
        {
            // Arrange
            var testDirectories = new[]
            {
                "Tests/Unit",
                "Tests/Integration", 
                "Tests/Performance",
                "Tests/Multiplayer",
                "Tests/Utils",
                "Tests/Mocks"
            };

            var testFiles = new[]
            {
                "Tests/TestConfig.cs",
                "Tests/Utils/TestHelpers.cs"
            };

            // Act & Assert
            foreach (var dir in testDirectories)
            {
                Assert.IsTrue(Directory.Exists(dir), $"Test directory {dir} should exist");
            }

            foreach (var file in testFiles)
            {
                Assert.IsTrue(File.Exists(file), $"Test infrastructure file {file} should exist");
            }
        }

        [Test]
        [Category(TestConfig.HighPriority)]
        public void CursorCommands_AreProperlyConfigured()
        {
            // Arrange
            var commandFiles = new[]
            {
                ".cursor/commands/execute-task.md",
                ".cursor/commands/create-tests.md",
                ".cursor/commands/run-tests.md",
                ".cursor/commands/README.md"
            };

            var ruleFiles = new[]
            {
                ".cursor/rules/commands-integration.mdc"
            };

            // Act & Assert
            Assert.IsTrue(Directory.Exists(".cursor/commands"), ".cursor/commands directory should exist");

            foreach (var file in commandFiles)
            {
                Assert.IsTrue(File.Exists(file), $"Command file {file} should exist");
                
                var content = File.ReadAllText(file);
                Assert.IsTrue(content.Length > 100, $"Command file {file} should have substantial content");
            }

            foreach (var file in ruleFiles)
            {
                Assert.IsTrue(File.Exists(file), $"Rule file {file} should exist");
            }
        }

        [Test]
        [Category(TestConfig.CriticalPriority)]
        public void PlanningDocuments_AreComplete()
        {
            // Arrange
            var planningFiles = new[]
            {
                "Planning/Development_Plan.md",
                "Planning/Features_Plan.md",
                "Planning/Testing_Plan.md",
                "Planning/Compatibility_Plan.md",
                "Planning/Performance_Plan.md",
                "Planning/Release_Plan.md",
                "Planning/Technical_Architecture.md",
                "Planning/Roadmap.md",
                "Planning/README.md"
            };

            // Act & Assert
            Assert.IsTrue(Directory.Exists("Planning"), "Planning directory should exist");

            foreach (var file in planningFiles)
            {
                Assert.IsTrue(File.Exists(file), $"Planning file {file} should exist");
                
                var content = File.ReadAllText(file);
                Assert.IsTrue(content.Length > 500, $"Planning file {file} should have substantial content");
                Assert.IsTrue(content.Contains("#"), $"Planning file {file} should contain markdown headers");
            }
        }

        [Test]
        [Category(TestConfig.HighPriority)]
        public void AboutXml_HasCorrectStructureAndMetadata()
        {
            // Arrange
            var aboutXmlPath = "About/About.xml";

            // Act
            Assert.IsTrue(File.Exists(aboutXmlPath), "About.xml should exist");
            
            var content = File.ReadAllText(aboutXmlPath);

            // Assert - Check XML structure
            Assert.IsTrue(content.Contains("<?xml"), "About.xml should start with XML declaration");
            Assert.IsTrue(content.Contains("<ModMetaData>"), "About.xml should contain ModMetaData root element");
            Assert.IsTrue(content.Contains("<packageId>rimasync.mod</packageId>"), 
                "About.xml should contain correct package ID");
            Assert.IsTrue(content.Contains("<name>RimAsync</name>"), 
                "About.xml should contain mod name");
            Assert.IsTrue(content.Contains("<supportedVersions>"), 
                "About.xml should specify supported versions");
            Assert.IsTrue(content.Contains("1.5"), 
                "About.xml should support RimWorld 1.5");
        }

        [Test]
        [Category(TestConfig.MediumPriority)]
        public void DocumentationFiles_ArePresent()
        {
            // Arrange
            var documentationFiles = new[]
            {
                "README.md",
                "CURSOR_COMMANDS.md"
            };

            // Act & Assert
            foreach (var file in documentationFiles)
            {
                Assert.IsTrue(File.Exists(file), $"Documentation file {file} should exist");
                
                var content = File.ReadAllText(file);
                Assert.IsTrue(content.Length > 200, $"Documentation file {file} should have substantial content");
                Assert.IsTrue(content.Contains("#"), $"Documentation file {file} should contain markdown headers");
            }
        }

        [Test]
        [Category(TestConfig.HighPriority)]
        public void ProjectHierarchy_IsConsistent()
        {
            // Arrange & Act
            var rootFiles = Directory.GetFiles(".", "*", SearchOption.TopDirectoryOnly);
            var sourceStructure = Directory.GetDirectories("Source", "*", SearchOption.AllDirectories);

            // Assert
            Assert.IsTrue(rootFiles.Any(f => f.Contains("README")), "Root should contain README file");
            Assert.IsTrue(sourceStructure.Any(d => d.Contains("RimAsync")), "Source should contain RimAsync folder structure");
            
            // Check that we don't have conflicting structures
            Assert.IsFalse(Directory.Exists("RimThreaded"), "Should not contain RimThreaded folder (conflict)");
            Assert.IsFalse(File.Exists("Source/RimAsync/RimThreaded.cs"), "Should not contain RimThreaded files");
        }
    }
} 