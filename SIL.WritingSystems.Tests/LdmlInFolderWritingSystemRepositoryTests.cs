using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Xml;
using System.Xml.Linq;
using NUnit.Framework;
using Palaso.TestUtilities;
using SIL.Keyboarding;
using SIL.WritingSystems.Migration.WritingSystemsLdmlV0To1Migration;

namespace SIL.WritingSystems.Tests
{
	[TestFixture]
	public class LdmlInFolderWritingSystemRepositoryInterfaceTests : WritingSystemRepositoryTests
	{
		private List<string> _testPaths;

		[SetUp]
		public override void SetUp()
		{
			_testPaths = new List<string>();
			base.SetUp();
		}

		[TearDown]
		public override void TearDown()
		{
			base.TearDown();
			foreach (string testPath in _testPaths)
			{
				if (Directory.Exists(testPath))
				{
					Directory.Delete(testPath, true);
				}
			}
		}

		public override IWritingSystemRepository CreateNewStore()
		{
			string testPath = Path.GetTempPath() + "PalasoTest" + _testPaths.Count;
			if (Directory.Exists(testPath))
			{
				Directory.Delete(testPath, true);
			}
			_testPaths.Add(testPath);
			LdmlInFolderWritingSystemRepository repository = LdmlInFolderWritingSystemRepository.Initialize(testPath, Enumerable.Empty<ICustomDataMapper>(),
				null, DummyWritingSystemHandler.OnMigration, DummyWritingSystemHandler.OnLoadProblem);
			//repository.DontAddDefaultDefinitions = true;
			return repository;
		}
	}

	[TestFixture]
	public class LdmlInFolderWritingSystemRepositoryTests
	{
		private class TestEnvironment : IDisposable
		{
			private readonly TemporaryFolder _localRepoFolder;
			private readonly WritingSystemDefinition _writingSystem;
			private readonly TemporaryFolder _sldrCacheFolder;
			private readonly TemporaryFolder _templateFolder;
			private readonly TemporaryFolder _globalRepoFolder;

			public TestEnvironment()
			{
				_localRepoFolder = new TemporaryFolder("LdmlInFolderWritingSystemRepositoryTests");
				_sldrCacheFolder = new TemporaryFolder("SldrCache");
				_templateFolder = new TemporaryFolder("Templates");
				_globalRepoFolder = new TemporaryFolder("GlobalWritingSystemRepository");
				_writingSystem = new WritingSystemDefinition();
				Reset();
			}

			public void Reset()
			{
				GlobalRepository = new GlobalWritingSystemRepository(_globalRepoFolder.Path);
				LocalRepository = new TestLdmlInFolderWritingSystemRepository(_localRepoFolder.Path, GlobalRepository) {TemplateFolder = _templateFolder.Path};
			}

			public void Dispose()
			{
				_globalRepoFolder.Dispose();
				_sldrCacheFolder.Dispose();
				_templateFolder.Dispose();
				_localRepoFolder.Dispose();
			}

			public TestLdmlInFolderWritingSystemRepository LocalRepository { get; private set; }

			public GlobalWritingSystemRepository GlobalRepository { get; private set; }

			public string LocalRepositoryPath
			{
				get { return _localRepoFolder.Path; }
			}

			public string SldrCachePath
			{
				get { return _sldrCacheFolder.Path; }
			}

			public WritingSystemDefinition WritingSystem
			{
				get { return _writingSystem; }
			}

			public string GetPathForLocalWSId(string id)
			{
				string path = Path.Combine(_localRepoFolder.Path, id + ".ldml");
				return path;
			}

			public string GetPathForGlobalWSId(string id)
			{
				string path = Path.Combine(GlobalWritingSystemRepository.CurrentVersionPath(_globalRepoFolder.Path), id + ".ldml");
				return path;
			}

			public void AssertWritingSystemFileExists(string id)
			{
				Assert.IsTrue(File.Exists(GetPathForLocalWSId(id)));
			}
		}

		[Test]
		public void LatestVersion_IsThree()
		{
			Assert.AreEqual(3, WritingSystemDefinition.LatestWritingSystemDefinitionVersion);
		}

		[Test]
		public void PathToCollection_SameAsGiven()
		{
			using (var environment = new TestEnvironment())
			{
				Assert.AreEqual(environment.LocalRepositoryPath, environment.LocalRepository.PathToWritingSystems);
			}
		}

		[Test]
		public void SaveDefinitionsThenLoad_CountEquals2()
		{
			using (var environment = new TestEnvironment())
			{
				environment.WritingSystem.Language = "one";
				environment.LocalRepository.SaveDefinition(environment.WritingSystem);
				var ws2 = new WritingSystemDefinition
					{
						Language = "two"
					};
				environment.LocalRepository.SaveDefinition(ws2);
				LdmlInFolderWritingSystemRepository newStore = LdmlInFolderWritingSystemRepository.Initialize(environment.LocalRepositoryPath, Enumerable.Empty<ICustomDataMapper>(),
					null, DummyWritingSystemHandler.OnMigration, DummyWritingSystemHandler.OnLoadProblem);

				Assert.AreEqual(2, newStore.Count);
			}
		}

		[Test]
		public void SavesWhenNotPreexisting()
		{
			using (var environment = new TestEnvironment())
			{
				environment.LocalRepository.SaveDefinition(environment.WritingSystem);
				environment.AssertWritingSystemFileExists(environment.WritingSystem.Id);
			}
		}

		[Test]
		public void SavesWhenPreexisting()
		{
			using (var environment = new TestEnvironment())
			{
				environment.WritingSystem.Language = "en";
				environment.LocalRepository.SaveDefinition(environment.WritingSystem);
				environment.LocalRepository.SaveDefinition(environment.WritingSystem);

				var ws2 = new WritingSystemDefinition();
				environment.WritingSystem.Language = "en";
				environment.LocalRepository.SaveDefinition(ws2);
			}
		}

		[Test]
		public void RegressionWhereUnchangedDefDeleted()
		{
			using (var environment = new TestEnvironment())
			{
				environment.WritingSystem.Language = "qaa";
				environment.LocalRepository.SaveDefinition(environment.WritingSystem);
				var ws2 = environment.LocalRepository.Get(environment.WritingSystem.StoreId);
				environment.LocalRepository.SaveDefinition(ws2);
				environment.AssertWritingSystemFileExists(environment.WritingSystem.Id);
			}
		}

		[Test]
		public void SavesWhenDirectoryNotFound()
		{
			using (var environment = new TestEnvironment())
			{
				string newRepoPath = Path.Combine(environment.LocalRepositoryPath, "newguy");
				var newRepository = new TestLdmlInFolderWritingSystemRepository(newRepoPath);
				newRepository.SaveDefinition(environment.WritingSystem);
				Assert.That(File.Exists(Path.Combine(newRepoPath, environment.WritingSystem.Id + ".ldml")));
			}
		}

		[Test]
		public void Save_WritingSystemIdChanged_ChangeLogUpdated()
		{
			using (var e = new TestEnvironment())
			{
				LdmlInFolderWritingSystemRepository repo = LdmlInFolderWritingSystemRepository.Initialize(Path.Combine(e.LocalRepositoryPath, "idchangedtest1"), Enumerable.Empty<ICustomDataMapper>(),
					null, DummyWritingSystemHandler.OnMigration, DummyWritingSystemHandler.OnLoadProblem);
				var ws = new WritingSystemDefinition("en");
				repo.Set(ws);
				repo.Save();

				ws.Script = "Latn";
				repo.Set(ws);
				ws.Script = "Thai";
				repo.Set(ws);

				var ws2 = new WritingSystemDefinition("de");
				repo.Set(ws2);
				ws2.Script = "Latn";
				repo.Save();

				string logFilePath = Path.Combine(repo.PathToWritingSystems, "idchangelog.xml");
				AssertThatXmlIn.File(logFilePath).HasAtLeastOneMatchForXpath("/WritingSystemChangeLog/Changes/Change/From[text()='en']");
				AssertThatXmlIn.File(logFilePath).HasAtLeastOneMatchForXpath("/WritingSystemChangeLog/Changes/Change/To[text()='en-Thai']");

				// writing systems added for the first time shouldn't be in the log as a change
				AssertThatXmlIn.File(logFilePath).HasNoMatchForXpath("/WritingSystemChangeLog/Changes/Change/From[text()='de']");
			}
		}

		[Test]
		public void Save_WritingSystemIdConflated_ChangeLogUpdatedAndDoesNotContainDelete()
		{
			using (var e = new TestEnvironment())
			{
				LdmlInFolderWritingSystemRepository repo = LdmlInFolderWritingSystemRepository.Initialize(Path.Combine(e.LocalRepositoryPath, "idchangedtest1"), Enumerable.Empty<ICustomDataMapper>(),
					null, DummyWritingSystemHandler.OnMigration, DummyWritingSystemHandler.OnLoadProblem);
				var ws = new WritingSystemDefinition("en");
				repo.Set(ws);
				repo.Save();

				var ws2 = new WritingSystemDefinition("de");
				repo.Set(ws2);
				repo.Save();

				repo.Conflate(ws.Id, ws2.Id);
				repo.Save();

				string logFilePath = Path.Combine(repo.PathToWritingSystems, "idchangelog.xml");
				AssertThatXmlIn.File(logFilePath).HasAtLeastOneMatchForXpath("/WritingSystemChangeLog/Changes/Merge/From[text()='en']");
				AssertThatXmlIn.File(logFilePath).HasAtLeastOneMatchForXpath("/WritingSystemChangeLog/Changes/Merge/To[text()='de']");
				AssertThatXmlIn.File(logFilePath).HasNoMatchForXpath("/WritingSystemChangeLog/Changes/Delete/Id[text()='en']");
			}
		}

		[Test]
		public void StoreIdAfterSave_SameAsFileNameWithoutExtension()
		{
			using (var environment = new TestEnvironment())
			{
				environment.WritingSystem.Language = "en";
				environment.LocalRepository.SaveDefinition(environment.WritingSystem);
				Assert.AreEqual("en", environment.WritingSystem.StoreId);
			}
		}

		[Test]
		public void StoreIdAfterLoad_SameAsFileNameWithoutExtension()
		{
			using (var environment = new TestEnvironment())
			{
				environment.WritingSystem.Language = "en";
				Assert.AreNotEqual(0, Directory.GetFiles(environment.LocalRepositoryPath, "*.ldml"));
				environment.LocalRepository.SaveDefinition(environment.WritingSystem);
				environment.Reset();
				WritingSystemDefinition ws2 = environment.LocalRepository.Get("en");
				Assert.AreEqual(
					Path.GetFileNameWithoutExtension(Directory.GetFiles(environment.LocalRepositoryPath, "*.ldml")[0]), ws2.StoreId);
			}
		}

		[Test]
		public void UpdatesFileNameWhenIsoChanges()
		{
			using (var environment = new TestEnvironment())
			{
				environment.WritingSystem.Language = "en";
				environment.LocalRepository.SaveDefinition(environment.WritingSystem);
				string path = Path.Combine(environment.LocalRepository.PathToWritingSystems, "en.ldml");
				Assert.IsTrue(File.Exists(path));
				var ws2 = environment.LocalRepository.Get(environment.WritingSystem.Id);
				ws2.Language = "de";
				Assert.AreEqual("en", ws2.StoreId);
				environment.LocalRepository.SaveDefinition(ws2);
				Assert.AreEqual("de", ws2.StoreId);
				Assert.IsFalse(File.Exists(path));
				path = Path.Combine(environment.LocalRepository.PathToWritingSystems, "de.ldml");
				Assert.IsTrue(File.Exists(path));
			}
		}

		[Test]
		public void MakesNewFileIfNeeded()
		{
			using (var environment = new TestEnvironment())
			{

				environment.WritingSystem.Language = "en";
				environment.LocalRepository.SaveDefinition(environment.WritingSystem);
				AssertThatXmlIn.File(environment.GetPathForLocalWSId(environment.WritingSystem.Id)).HasAtLeastOneMatchForXpath("ldml/identity/language[@type='en']");
			}
		}

		[Test]
		public void CanAddVariantToLdmlUsingSameWS()
		{
			using (var environment = new TestEnvironment())
			{
				environment.LocalRepository.SaveDefinition(environment.WritingSystem);
				environment.WritingSystem.Variants.Add("1901");
				environment.LocalRepository.SaveDefinition(environment.WritingSystem);
				AssertThatXmlIn.File(environment.GetPathForLocalWSId(environment.WritingSystem.Id)).HasAtLeastOneMatchForXpath("ldml/identity/variant[@type='1901']");
			}
		}

		[Test]
		public void CanAddVariantToExistingLdml()
		{
			using (var environment = new TestEnvironment())
			{

				environment.WritingSystem.Language = "en";
				environment.WritingSystem.Script = "Latn";
				environment.WritingSystem.Abbreviation = "bl";
					//crucially, abbreviation isn't part of the name of the file
				environment.LocalRepository.SaveDefinition(environment.WritingSystem);

				environment.Reset();
				WritingSystemDefinition ws2 = environment.LocalRepository.Get(environment.WritingSystem.StoreId);
				ws2.Variants.Add("piglatin");
				environment.LocalRepository.SaveDefinition(ws2);
				string path = Path.Combine(environment.LocalRepository.PathToWritingSystems,
										   environment.GetPathForLocalWSId(ws2.Id));
				AssertThatXmlIn.File(path).HasAtLeastOneMatchForXpath("ldml/identity/variant[@type='x-piglatin']");

				// TODO: Add this back when Abbreviation is written to application-specific namespace
#if WS_FIX
				AssertThatXmlIn.File(path).HasAtLeastOneMatchForXpath("ldml/special/palaso:abbreviation[@value='bl']",
																	  environment.NamespaceManager);
#endif
			}
		}

		[Test]
		public void CanReadVariant()
		{
			using (var environment = new TestEnvironment())
			{
				environment.WritingSystem.Language = "en";
				environment.WritingSystem.Variants.Add("piglatin");
				environment.LocalRepository.SaveDefinition(environment.WritingSystem);

				environment.Reset();
				WritingSystemDefinition ws2 = environment.LocalRepository.Get(environment.WritingSystem.StoreId);
				Assert.That(ws2.Variants, Is.EqualTo(new VariantSubtag[] {"piglatin"}));
			}
		}

		// TODO: Add this when DefaultFontName is written to application-specific
#if WS_FIX
		[Test]
		public void CanSaveAndReadDefaultFont()
		{
			using (var environment = new TestEnvironment())
			{
				environment.WritingSystem.Language = "en";
				environment.WritingSystem.DefaultFontName = "Courier";
				environment.Collection.SaveDefinition(environment.WritingSystem);

				var newCollection = LdmlInFolderWritingSystemRepository.Initialize(environment.TestPath, DummyWritingSystemHandler.OnMigration, DummyWritingSystemHandler.OnLoadProblem);
				var ws2 = newCollection.Get("en");
				Assert.AreEqual("Courier", ws2.DefaultFontName);
			}
		}
#endif

		[Test]
		public void CanSaveAndReadKeyboardId()
		{
			using (var environment = new TestEnvironment())
			{
				environment.WritingSystem.Language = "en";
				var kbd1 = new DefaultKeyboardDefinition("Thai", "Thai");
				kbd1.Format = KeyboardFormat.Msklc;
				environment.WritingSystem.KnownKeyboards.Add(kbd1);
				environment.LocalRepository.SaveDefinition(environment.WritingSystem);

				environment.Reset();
				WritingSystemDefinition ws2 = environment.LocalRepository.Get("en");
				Assert.AreEqual("Thai", ws2.KnownKeyboards[0].Id);
			}
		}

		[Test]
		public void CanSaveAndReadRightToLeft()
		{
			using (var environment = new TestEnvironment())
			{
				environment.WritingSystem.Language = "en";
				Assert.IsFalse(environment.WritingSystem.RightToLeftScript);
				environment.WritingSystem.RightToLeftScript = true;
				Assert.IsTrue(environment.WritingSystem.RightToLeftScript);
				environment.LocalRepository.SaveDefinition(environment.WritingSystem);

				environment.Reset();
				WritingSystemDefinition ws2 = environment.LocalRepository.Get("en");
				Assert.IsTrue(ws2.RightToLeftScript);
			}
		}

		// TODO: Does IsUnicodeEncoded go away or get put in application-specific?
#if WS_FIX
		[Test]
		public void CanSaveAndReadIsUnicode()
		{
			using (var environment = new TestEnvironment())
			{
				environment.WritingSystem.Language = "en";
				Assert.IsTrue(environment.WritingSystem.IsUnicodeEncoded);
				environment.WritingSystem.IsUnicodeEncoded = false;
				Assert.IsFalse(environment.WritingSystem.IsUnicodeEncoded);
				environment.Collection.SaveDefinition(environment.WritingSystem);

				var newCollection = LdmlInFolderWritingSystemRepository.Initialize(environment.TestPath, DummyWritingSystemHandler.OnMigration, DummyWritingSystemHandler.OnLoadProblem);
				var ws2 = newCollection.Get("en");
				Assert.IsFalse(ws2.IsUnicodeEncoded);
			}
		}
#endif

		[Test]
		public void CanRemoveVariant()
		{
			using (var environment = new TestEnvironment())
			{
				environment.WritingSystem.Language = "en";
				environment.WritingSystem.Variants.Add("piglatin");
				environment.LocalRepository.SaveDefinition(environment.WritingSystem);
				string path = environment.GetPathForLocalWSId(environment.WritingSystem.Id);

				AssertThatXmlIn.File(path).HasAtLeastOneMatchForXpath("ldml/identity/variant");
				environment.WritingSystem.Variants.Clear();
				environment.LocalRepository.SaveDefinition(environment.WritingSystem);
				AssertThatXmlIn.File(environment.GetPathForLocalWSId(environment.WritingSystem.Id)).HasNoMatchForXpath("ldml/identity/variant");
			}
		}

		// TODO: Abbreviation to go in application-specific
#if WS_FIX
		[Test]
		public void CanRemoveAbbreviation()
		{
			using (var environment = new TestEnvironment())
			{
				environment.WritingSystem.Language = "en";
				environment.WritingSystem.Abbreviation = "abbrev";
				environment.Collection.SaveDefinition(environment.WritingSystem);
				string path = environment.GetPathForWsId(environment.WritingSystem.LanguageTag);
				AssertThatXmlIn.File(path).HasAtLeastOneMatchForXpath(
					"ldml/special/palaso:abbreviation[@value='abbrev']",
					environment.NamespaceManager
				);
				environment.WritingSystem.Abbreviation = string.Empty;
				environment.Collection.SaveDefinition(environment.WritingSystem);
				AssertThatXmlIn.File(path).HasAtLeastOneMatchForXpath(
					"ldml/special/palaso:abbreviation[@value='en']",
					environment.NamespaceManager
				);
			}
		}

		[Test]
		public void WritesAbbreviationToLdml()
		{

			using (var environment = new TestEnvironment())
			{
				environment.WritingSystem.Language = "en";
				environment.WritingSystem.Abbreviation = "bl";
				environment.Collection.SaveDefinition(environment.WritingSystem);
				AssertThatXmlIn.File(environment.GetPathForWsId(environment.WritingSystem.LanguageTag)).HasAtLeastOneMatchForXpath(
					"ldml/special/palaso:abbreviation[@value='bl']", environment.NamespaceManager);
			}
		}
#endif

		[Test]
		public void CanDeleteFileThatIsNotInTrash()
		{
			using (var environment = new TestEnvironment())
			{
				environment.WritingSystem.Language = "en";
				environment.LocalRepository.SaveDefinition(environment.WritingSystem);
				string path = Path.Combine(environment.LocalRepository.PathToWritingSystems,
										   environment.GetPathForLocalWSId(environment.WritingSystem.Id));
				Assert.IsTrue(File.Exists(path));
				environment.LocalRepository.Remove(environment.WritingSystem.Language);
				Assert.IsFalse(File.Exists(path));
				AssertFileIsInTrash(environment);
			}
		}

		private static void AssertFileIsInTrash(TestEnvironment environment)
		{
			string path = Path.Combine(environment.LocalRepository.PathToWritingSystems, "trash");
			path = Path.Combine(path,environment.WritingSystem.Id + ".ldml");
			Assert.IsTrue(File.Exists(path));
		}

		[Test]
		public void CanDeleteFileMatchingOneThatWasPreviouslyTrashed()
		{
			using (var environment = new TestEnvironment())
			{
				environment.WritingSystem.Language = "en";
				environment.LocalRepository.SaveDefinition(environment.WritingSystem);
				environment.LocalRepository.Remove(environment.WritingSystem.StoreId);
				AssertFileIsInTrash(environment);
				var ws2 = new WritingSystemDefinition {Language = "en"};
				environment.LocalRepository.SaveDefinition(ws2);
				environment.LocalRepository.Remove(ws2.StoreId);
				string path = Path.Combine(environment.LocalRepository.PathToWritingSystems,
										   environment.GetPathForLocalWSId(environment.WritingSystem.Id));
				Assert.IsFalse(File.Exists(path));
				AssertFileIsInTrash(environment);
			}
		}

		[Test]
		public void MarkedNotModifiedWhenNew()
		{
			using (var environment = new TestEnvironment())
			{
				//not worth saving until has some data
				Assert.IsFalse(environment.WritingSystem.IsChanged);
			}
		}

		[Test]
		public void MarkedAsModifiedWhenIsoChanges()
		{
			using (var environment = new TestEnvironment())
			{
				environment.WritingSystem.Language = "en";
				Assert.IsTrue(environment.WritingSystem.IsChanged);
			}
		}

		[Test]
		public void MarkedAsNotModifiedWhenLoaded()
		{
			using (var environment = new TestEnvironment())
			{
				environment.WritingSystem.Language = "en";
				environment.LocalRepository.SaveDefinition(environment.WritingSystem);
				environment.Reset();
				WritingSystemDefinition ws2 = environment.LocalRepository.Get(environment.WritingSystem.StoreId);
				Assert.IsFalse(ws2.IsChanged);
			}
		}

		[Test]
		public void MarkedAsNotModifiedWhenSaved()
		{
			using (var environment = new TestEnvironment())
			{
				environment.WritingSystem.Language = "en";
				Assert.IsTrue(environment.WritingSystem.IsChanged);
				environment.LocalRepository.SaveDefinition(environment.WritingSystem);
				Assert.IsFalse(environment.WritingSystem.IsChanged);
				environment.WritingSystem.Language = "de";
				Assert.IsTrue(environment.WritingSystem.IsChanged);
			}
		}

		[Test]
		public void SystemWritingSystemProvider_Set_WritingSystemsAreIncludedInStore()
		{
			using (var environment = new TestEnvironment())
			{
				environment.LocalRepository.SystemWritingSystemProvider = new DummyWritingSystemProvider();
				var list = environment.LocalRepository.AllWritingSystems;
				Assert.IsTrue(ContainsLanguageWithName(list, "English"));
			}
		}

		[Test]
		public void DefaultLanguageNotAddedIfInTrash()
		{
			using (var environment = new TestEnvironment())
			{
				environment.LocalRepository.SystemWritingSystemProvider = new DummyWritingSystemProvider();
				var list = environment.LocalRepository.AllWritingSystems;
				Assert.IsTrue(ContainsLanguageWithName(list, "English"));
				var list2 = new List<WritingSystemDefinition>(environment.LocalRepository.AllWritingSystems);
				WritingSystemDefinition ws2 = list2[0];
				environment.LocalRepository.Remove(ws2.Language);

				environment.Reset();
				//  repository.DontAddDefaultDefinitions = false;
				environment.LocalRepository.SystemWritingSystemProvider = new DummyWritingSystemProvider();
				Assert.IsFalse(ContainsLanguageWithName(environment.LocalRepository.AllWritingSystems, "English"));
			}

		}

		[Test]
		public void Constructor_LdmlFolderStoreContainsMultipleFilesThatOnLoadDescribeWritingSystemsWithIdenticalRFC5646Tags_Throws()
		{
			using (var environment = new TestEnvironment())
			{
				File.WriteAllText(Path.Combine(environment.LocalRepositoryPath, "de-Zxxx-x-audio.ldml"),
								  LdmlContentForTests.CurrentVersion("de", WellKnownSubtags.UnwrittenScript, "", "x-audio"));
				File.WriteAllText(Path.Combine(environment.LocalRepositoryPath, "inconsistent-filename.ldml"),
								  LdmlContentForTests.CurrentVersion("de", WellKnownSubtags.UnwrittenScript, "", "x-audio"));

				environment.Reset();
				IList<WritingSystemRepositoryProblem> problems = environment.LocalRepository.LoadProblems;

				Assert.That(problems.Count, Is.EqualTo(2));
				Assert.That(
					problems[0].Exception,
					Is.TypeOf<ApplicationException>().With.Property("Message").
					ContainsSubstring(String.Format(
						@"The writing system file {0} seems to be named inconsistently. It contains the IETF language tag: 'de-Zxxx-x-audio'. The name should have been made consistent with its content upon migration of the writing systems.",
						Path.Combine(environment.LocalRepositoryPath, "inconsistent-filename.ldml")
					))
				);
				Assert.That(
					problems[1].Exception,
					Is.TypeOf<ArgumentException>().With.Property("Message").
					ContainsSubstring("Unable to set writing system 'de-Zxxx-x-audio' because this id already exists. Please change this writing system id before setting it.")
				);

			}
		}

		[Test]
		public void Conflate_ChangelogRecordsChange()
		{
			using(var e = new TestEnvironment())
			{
				e.LocalRepository.Set(new WritingSystemDefinition("de"));
				e.LocalRepository.Set(new WritingSystemDefinition("en"));
				e.LocalRepository.Conflate("de", "en");
				Assert.That(e.LocalRepository.WritingSystemIdHasChangedTo("de"), Is.EqualTo("en"));
			}
		}

		[Test]
		//This is not really a problem, but it would be nice if the file were made consistant. So make we will make them run it through the migrator, which they should be using anyway.
		public void Constructor_LdmlFolderStoreContainsInconsistentlyNamedFile_HasExpectedProblem()
		{
			using (var environment = new TestEnvironment())
			{
				File.WriteAllText(Path.Combine(environment.LocalRepositoryPath, "tpi-Zxxx-x-audio.ldml"),
								  LdmlContentForTests.CurrentVersion("de", "latn", "ch", "1901"));

				environment.Reset();
				IList<WritingSystemRepositoryProblem> problems = environment.LocalRepository.LoadProblems;
				Assert.That(problems.Count, Is.EqualTo(1));
				Assert.That(
					problems[0].Exception,
					Is.TypeOf<ApplicationException>().With.Property("Message").
					ContainsSubstring(String.Format(
						@"The writing system file {0} seems to be named inconsistently. It contains the IETF language tag: 'de-latn-ch-1901'. The name should have been made consistent with its content upon migration of the writing systems.",
						Path.Combine(environment.LocalRepositoryPath, "tpi-Zxxx-x-audio.ldml")
					))
				);
			}
		}

		[Test]
		public void Constructor_LdmlFolderStoreContainsInconsistentlyNamedFileDifferingInCaseOnly_HasNoProblem()
		{
			using (var environment = new TestEnvironment())
			{
				File.WriteAllText(Path.Combine(environment.LocalRepositoryPath, "aa-latn.ldml"),
								  LdmlContentForTests.CurrentVersion("aa", "Latn", "", ""));

				environment.Reset();
				IList<WritingSystemRepositoryProblem> problems = environment.LocalRepository.LoadProblems;
				Assert.That(problems.Count, Is.EqualTo(0));
			}
		}

		// TODO: Add when migrating FlexPrivateUse
#if WS_FIX
		[Test]
		public void Set_WritingSystemWasLoadedFromFlexPrivateUseLdmlAndRearranged_DoesNotChangeFileName()
		{
			using (var environment = new TestEnvironment())
			{
				var pathToFlexprivateUseLdml = Path.Combine(environment.TestPath, "x-en-Zxxx-x-audio.ldml");
				File.WriteAllText(pathToFlexprivateUseLdml,
								  LdmlContentForTests.Version0("x-en", "Zxxx", "", "x-audio"));
				environment.Collection = LdmlInFolderWritingSystemRepository.Initialize(environment.TestPath, DummyWritingSystemHandler.OnMigration, DummyWritingSystemHandler.OnLoadProblem, WritingSystemCompatibility.Flex7V0Compatible);
				var ws = environment.Collection.Get("x-en-Zxxx-x-audio");
				environment.Collection.Set(ws);
				Assert.That(File.Exists(pathToFlexprivateUseLdml), Is.True);
			}
		}
#endif

		[Test]
		//this used to throw
		public void LoadAllDefinitions_FilenameDoesNotMatchRfc5646Tag_NoProblem()
		{
			using (var environment = new TestEnvironment())
			{
				//Make the filepath inconsistant
				environment.WritingSystem.Language = "en";
				environment.LocalRepository.SaveDefinition(environment.WritingSystem);
				File.Move(Path.Combine(environment.LocalRepositoryPath, "en.ldml"), Path.Combine(environment.LocalRepositoryPath, "de.ldml"));

				// Now try to load up.
				environment.Reset();
				Assert.That(environment.LocalRepository.Contains("en"));
			}
		}

		[Test]
		public void Get_WritingSystemContainedInFileWithfilenameThatDoesNotMatchRfc5646Tag_ReturnsWritingSystem()
		{
			using (var environment = new TestEnvironment())
			{
				//Make the filepath inconsistant
				environment.WritingSystem.Language = "en";
				environment.LocalRepository.SaveDefinition(environment.WritingSystem);
				File.Move(Path.Combine(environment.LocalRepositoryPath, "en.ldml"), Path.Combine(environment.LocalRepositoryPath, "de.ldml"));

				// Now try to load up.
				environment.Reset();
				var ws = environment.LocalRepository.Get("en");
				Assert.That(ws.Id, Is.EqualTo("en"));
			}
		}

#if WS_FIX
		[Test]
		public void LoadAllDefinitions_FilenameIsFlexConformPrivateUseAndDoesNotMatchRfc5646TagWithLegacySupport_DoesNotThrow()
		{
			using (var environment = new TestEnvironment())
			{
				var ldmlPath = Path.Combine(environment.TestPath, "x-en-Zxxx.ldml");
				File.WriteAllText(ldmlPath, LdmlContentForTests.Version0("x-en", "Zxxx", "", ""));
				var repo = LdmlInFolderWritingSystemRepository.Initialize(environment.TestPath, DummyWritingSystemHandler.OnMigration, DummyWritingSystemHandler.OnLoadProblem, WritingSystemCompatibility.Flex7V0Compatible);

				// Now try to load up.
				Assert.That(repo.Get("x-en-Zxxx").Language, Is.EqualTo(new LanguageSubtag("en", true)));
			}
		}
#endif

		[Test]
		public void LoadAllDefinitions_FilenameIsFlexConformPrivateUseAndDoesNotMatchRfc5646Tag_Migrates()
		{
			using (var environment = new TestEnvironment())
			{
				var ldmlPath = Path.Combine(environment.LocalRepositoryPath, "x-en-Zxxx.ldml");
				File.WriteAllText(ldmlPath, LdmlContentForTests.Version0("x-en", "Zxxx", "", ""));
				LdmlInFolderWritingSystemRepository repo = LdmlInFolderWritingSystemRepository.Initialize(environment.LocalRepositoryPath, Enumerable.Empty<ICustomDataMapper>(),
					null, DummyWritingSystemHandler.OnMigration, DummyWritingSystemHandler.OnLoadProblem);

				// Now try to load up.
				Assert.That(repo.Get("qaa-Zxxx-x-en").Language, Is.EqualTo(new LanguageSubtag("en")));
			}
		}

		[Test]
		public void Set_NewWritingSystem_StoreContainsWritingSystem()
		{
			using (var environment = new TestEnvironment())
			{
				var ws = new WritingSystemDefinition("en");
				environment.LocalRepository.Set(ws);
				Assert.That(environment.LocalRepository.Get("en").Id, Is.EqualTo("en"));
			}
		}

#if WS_FIX
		[Test]
		public void SaveDefinition_WritingSystemCameFromFlexPrivateUseLdml_FileNameIsRetained()
		{
			using (var environment = new TestEnvironment())
			{
				var pathToFlexprivateUseLdml = Path.Combine(environment.TestPath, "x-Zxxx-x-audio.ldml");
				File.WriteAllText(pathToFlexprivateUseLdml, LdmlContentForTests.Version0("x", "Zxxx", "", "x-audio"));
				environment.Collection = LdmlInFolderWritingSystemRepository.Initialize(environment.TestPath, DummyWritingSystemHandler.OnMigration, DummyWritingSystemHandler.OnLoadProblem, WritingSystemCompatibility.Flex7V0Compatible);
				var ws = environment.Collection.Get("x-Zxxx-x-audio");
				environment.Collection.SaveDefinition(ws);
				Assert.That(File.Exists(pathToFlexprivateUseLdml));
			}
		}
#endif

		[Test]
		public void SaveDefinition_WritingSystemCameFromValidRfc5646WritingSystemStartingWithX_FileNameIsChanged()
		{
			using (var environment = new TestEnvironment())
			{
				var pathToFlexprivateUseLdml = Path.Combine(environment.LocalRepositoryPath, "x-Zxxx-x-audio.ldml");
				File.WriteAllText(pathToFlexprivateUseLdml, LdmlContentForTests.CurrentVersion("xh", "", "", ""));
				environment.Reset();
				Assert.That(File.Exists(Path.Combine(environment.LocalRepositoryPath, "xh.ldml")));
			}
		}

		[Test]
		public void WritingSystemIdHasBeenChanged_IdNeverExisted_ReturnsFalse()
		{
			using (var environment = new TestEnvironment())
			{
				//Add a writing system to the repo
				Assert.That(environment.LocalRepository.WritingSystemIdHasChanged("en"), Is.False);
			}
		}

		[Test]
		public void WritingSystemIdHasBeenChanged_IdChanged_ReturnsTrue()
		{
			using (var environment = new TestEnvironment())
			{
				//Add a writing system to the repo
				var ws = new WritingSystemDefinition("en");
				environment.LocalRepository.Set(ws);
				environment.LocalRepository.Save();
				//Now change the Id
				ws.Variants.Add("bogus");
				environment.LocalRepository.Save();
				Assert.That(environment.LocalRepository.WritingSystemIdHasChanged("en"), Is.True);
			}
		}

		[Test]
		public void WritingSystemIdHasBeenChanged_IdChangedToMultipleDifferentNewIds_ReturnsTrue()
		{
			using (var environment = new TestEnvironment())
			{
				//Add a writing system to the repo
				var wsEn = new WritingSystemDefinition("en");
				environment.LocalRepository.Set(wsEn);
				environment.LocalRepository.Save();
				//Now change the Id and create a duplicate of the original Id
				wsEn.Variants.Add("bogus");
				environment.LocalRepository.Set(wsEn);
				var wsEnDup = new WritingSystemDefinition("en");
				environment.LocalRepository.Set(wsEnDup);
				environment.LocalRepository.Save();
				//Now change the duplicate's Id as well
				wsEnDup.Variants.Add("bogus2");
				environment.LocalRepository.Set(wsEnDup);
				environment.LocalRepository.Save();
				Assert.That(environment.LocalRepository.WritingSystemIdHasChanged("en"), Is.True);
			}
		}

		[Test]
		public void WritingSystemIdHasBeenChanged_IdExistsAndHasNeverChanged_ReturnsFalse()
		{
			using (var environment = new TestEnvironment())
			{
				//Add a writing system to the repo
				var ws = new WritingSystemDefinition("en");
				environment.LocalRepository.Set(ws);
				environment.LocalRepository.Save();
				Assert.That(environment.LocalRepository.WritingSystemIdHasChanged("en"), Is.False);
			}
		}

		[Test]
		public void WritingSystemIdHasChangedTo_IdNeverExisted_ReturnsNull()
		{
			using (var environment = new TestEnvironment())
			{
				//Add a writing system to the repo
				Assert.That(environment.LocalRepository.WritingSystemIdHasChangedTo("en"), Is.Null);
			}
		}

		[Test]
		public void WritingSystemIdHasChangedTo_IdChanged_ReturnsNewId()
		{
			using (var environment = new TestEnvironment())
			{
				//Add a writing system to the repo
				var ws = new WritingSystemDefinition("en");
				environment.LocalRepository.Set(ws);
				environment.LocalRepository.Save();
				//Now change the Id
				ws.Variants.Add("bogus");
				environment.LocalRepository.Save();
				Assert.That(environment.LocalRepository.WritingSystemIdHasChangedTo("en"), Is.EqualTo("en-x-bogus"));
			}
		}

		[Test]
		public void WritingSystemIdHasChangedTo_IdChangedToMultipleDifferentNewIds_ReturnsNull()
		{
			using (var environment = new TestEnvironment())
			{
				//Add a writing system to the repo
				var wsEn = new WritingSystemDefinition("en");
				environment.LocalRepository.Set(wsEn);
				environment.LocalRepository.Save();
				//Now change the Id and create a duplicate of the original Id
				wsEn.Variants.Add("bogus");
				environment.LocalRepository.Set(wsEn);
				var wsEnDup = new WritingSystemDefinition("en");
				environment.LocalRepository.Set(wsEnDup);
				environment.LocalRepository.Save();
				//Now change the duplicate's Id as well
				wsEnDup.Variants.Add("bogus2");
				environment.LocalRepository.Set(wsEnDup);
				environment.LocalRepository.Save();
				Assert.That(environment.LocalRepository.WritingSystemIdHasChangedTo("en"), Is.Null);
			}
		}

		[Test]
		public void WritingSystemIdHasChangedTo_IdExistsAndHasNeverChanged_ReturnsId()
		{
			using (var environment = new TestEnvironment())
			{
				//Add a writing system to the repo
				var ws = new WritingSystemDefinition("en");
				environment.LocalRepository.Set(ws);
				environment.LocalRepository.Save();
				Assert.That(environment.LocalRepository.WritingSystemIdHasChangedTo("en"), Is.EqualTo("en"));
			}
		}

		[Test]
		//This test checks that "old" ldml files are not overwritten on Save before they can be used to roundtrip unknown ldml (i.e. from flex)
		public void Save_IdOfWsIsSameAsOldIdOfOtherWs_LdmlUnknownToRepoIsMaintained()
		{
			using (var environment = new TestEnvironment())
			{
				string germanFromFlex =
#region fileContent
 @"<?xml version='1.0' encoding='utf-8'?>
<ldml>
	<identity>
		<version number='' />
		<generation date='2010-12-02T23:05:33' />
		<language type='de' />
	</identity>
	<collations />
	<special xmlns:palaso='urn://palaso.org/ldmlExtensions/v1'>
		<palaso:abbreviation value='de' />
		<palaso:defaultFontFamily value='FieldWorks Test SIL' />
		<palaso:defaultKeyboard value='FwTest' />
		<palaso:languageName value='German' />
		<palaso:spellCheckingId value='de' />
		<palaso:version value='1'/>
	</special>
	<special xmlns:fw='urn://fieldworks.sil.org/ldmlExtensions/v1'>
		<fw:graphiteEnabled value='False' />
		<fw:windowsLCID value='1058' />
	</special>
</ldml>".Replace("'", "\"");
#endregion
				var pathForFlexGerman = Path.Combine(environment.LocalRepositoryPath, "de.ldml");
				var ws1 = new WritingSystemDefinition("en");
				var ws2 = new WritingSystemDefinition("de");
				//Create repo with english and flex german
				environment.LocalRepository.Set(ws1);
				environment.LocalRepository.Set(ws2);
				environment.LocalRepository.Save();
				//The content of the file is switched out here as opposed to loading from this content in the first place
				//because order is extremely important for this test and if we loaded from this ldml "de" would be the
				//first writing system in the repo rather than the second.
				File.WriteAllText(pathForFlexGerman, germanFromFlex);
				//rename the ws
				ws1.Language = "de";
				ws2.Language = "fr";
				environment.LocalRepository.Set(ws2);
				environment.LocalRepository.Set(ws1);
				environment.LocalRepository.Save();

				pathForFlexGerman = Path.Combine(environment.LocalRepositoryPath, "fr.ldml");
				var manager = new XmlNamespaceManager(new NameTable());
				manager.AddNamespace("fw", "urn://fieldworks.sil.org/ldmlExtensions/v1");
				AssertThatXmlIn.File(pathForFlexGerman).HasAtLeastOneMatchForXpath("/ldml/special/fw:graphiteEnabled", manager);
			}
		}

		[Test]
		public void Save_IdOfWsIsSameAsOldIdOfOtherWs_RepoHasCorrectNumberOfWritingSystemsOnLoad()
		{
			using (var environment = new TestEnvironment())
			{
				var ws1 = new WritingSystemDefinition("en");
				var ws2 = new WritingSystemDefinition("de");
				environment.LocalRepository.Set(ws1);
				environment.LocalRepository.Set(ws2);
				environment.LocalRepository.Save();
				//rename the ws
				ws1.Language = "de";
				ws2.Language = "fr";
				environment.LocalRepository.Set(ws2);
				environment.LocalRepository.Set(ws1);
				environment.LocalRepository.Save();
				environment.Reset();
				Assert.That(environment.LocalRepository.Count, Is.EqualTo(2));
			}
		}

		[Test]
		public void LoadAllDefinitions_LDMLV0_HasExpectedProblem()
		{
			using (var environment = new TestEnvironment())
			{
				var ldmlPath = Path.Combine(environment.LocalRepositoryPath, "en.ldml");
				File.WriteAllText(ldmlPath, LdmlContentForTests.Version0("en", "", "", ""));

				var repository = new LdmlInFolderWritingSystemRepository(environment.LocalRepositoryPath);
				var problems = repository.LoadProblems;

				Assert.That(problems.Count, Is.EqualTo(1));
				Assert.That(
					problems[0].Exception,
					Is.TypeOf<ApplicationException>().With.Property("Message").
					ContainsSubstring(String.Format(
						"The LDML tag 'en' is version 0.  Version {0} was expected.",
						WritingSystemDefinition.LatestWritingSystemDefinitionVersion
					))
				);
			}
		}

		[Test]
		public void LoadDefinitions_ValidLanguageTagStartingWithXButV0_Throws()
		{
			using (var environment = new TestEnvironment())
			{
				var pathToFlexprivateUseLdml = Path.Combine(environment.LocalRepositoryPath, "xh.ldml");
				File.WriteAllText(pathToFlexprivateUseLdml, LdmlContentForTests.Version0("xh", "", "", ""));
				var repository = new LdmlInFolderWritingSystemRepository(environment.LocalRepositoryPath);
				var problems = repository.LoadProblems;

				Assert.That(problems.Count, Is.EqualTo(1));
				Assert.That(
					problems[0].Exception,
					Is.TypeOf<ApplicationException>().With.Property("Message").
					ContainsSubstring(String.Format(
						"The LDML tag 'xh' is version 0.  Version {0} was expected.",
						WritingSystemDefinition.LatestWritingSystemDefinitionVersion
					))
				);
			}
		}

		[Test]
		public void CreateNew_TemplateAvailableInLocalRepo_UsedTemplateFromLocalRepo()
		{
			using (var environment = new TestEnvironment())
			{
				File.WriteAllText(environment.GetPathForLocalWSId("en"), @"<?xml version='1.0' encoding='utf-8'?>
<ldml>
	<identity>
		<version number='1.0'>From Repo</version>
		<generation date='0001-01-01T00:00:00' />
		<language type='en' />
		<script type='Latn' />
	</identity>
	<layout>
		<orientation>
			<characterOrder>left-to-right</characterOrder>
			<lineOrder>top-to-bottom</lineOrder>
		</orientation>
	</layout>
</ldml>
");
				environment.Reset();
				WritingSystemDefinition enWs = environment.LocalRepository.CreateNew("en");
				Assert.That(enWs.Language, Is.EqualTo((LanguageSubtag) "en"));
				Assert.That(enWs.Script, Is.EqualTo((ScriptSubtag) "Latn"));
				Assert.That(enWs.VersionDescription, Is.EqualTo("From Repo"));
				Assert.That(enWs.Template, Is.EqualTo(environment.GetPathForLocalWSId("en")));

				// ensure that the template is used when the writing system is saved
				enWs.Region = "US";
				environment.LocalRepository.Set(enWs);
				environment.LocalRepository.Save();
				XElement ldmlElem = XElement.Load(environment.GetPathForLocalWSId("en-US"));
				Assert.That((string) ldmlElem.Elements("layout").Elements("orientation").Elements("lineOrder").First(), Is.EqualTo("top-to-bottom"));
			}
		}

		[Test]
		public void CreateNew_TemplateAvailableInSldr_UsedTemplateFromSldr()
		{
			using (var environment = new TestEnvironment())
			{
				environment.LocalRepository.SldrLdmls["en"] = @"<?xml version='1.0' encoding='utf-8'?>
<ldml>
	<identity>
		<version number='1.0'>From SLDR</version>
		<generation date='0001-01-01T00:00:00' />
		<language type='en' />
		<script type='Latn' />
	</identity>
	<layout>
		<orientation>
			<characterOrder>left-to-right</characterOrder>
			<lineOrder>top-to-bottom</lineOrder>
		</orientation>
	</layout>
</ldml>
";

				WritingSystemDefinition enWs = environment.LocalRepository.CreateNew("en");
				Assert.That(enWs.Language, Is.EqualTo((LanguageSubtag) "en"));
				Assert.That(enWs.Script, Is.EqualTo((ScriptSubtag) "Latn"));
				Assert.That(enWs.VersionDescription, Is.EqualTo("From SLDR"));
				Assert.That(enWs.Template, Is.EqualTo(Path.Combine(environment.SldrCachePath, "en.ldml")));

				// ensure that the template is used when the writing system is saved
				environment.LocalRepository.Set(enWs);
				environment.LocalRepository.Save();
				XElement ldmlElem = XElement.Load(environment.GetPathForLocalWSId("en"));
				Assert.That((string) ldmlElem.Elements("layout").Elements("orientation").Elements("lineOrder").First(), Is.EqualTo("top-to-bottom"));
			}
		}

		[Test]
		public void CreateNew_TemplateNotAvailableInSldr_UsedTemplateFromTemplateFolder()
		{
			using (var environment = new TestEnvironment())
			{
				File.WriteAllText(Path.Combine(environment.LocalRepository.TemplateFolder, "zh-Hans-CN.ldml"), @"<?xml version='1.0' encoding='utf-8'?>
<ldml>
	<identity>
		<version number='1.0'>From Templates</version>
		<generation date='0001-01-01T00:00:00' />
		<language type='zh' />
		<script type='Hans' />
		<territory type='CN' />
	</identity>
	<layout>
		<orientation>
			<characterOrder>left-to-right</characterOrder>
			<lineOrder>top-to-bottom</lineOrder>
		</orientation>
	</layout>
</ldml>
");

				WritingSystemDefinition chWs = environment.LocalRepository.CreateNew("zh-Hans-CN");
				Assert.That(chWs.Language, Is.EqualTo((LanguageSubtag) "zh"));
				Assert.That(chWs.Script, Is.EqualTo((ScriptSubtag) "Hans"));
				Assert.That(chWs.Region, Is.EqualTo((RegionSubtag) "CN"));
				Assert.That(chWs.VersionDescription, Is.EqualTo("From Templates"));
				Assert.That(chWs.Template, Is.EqualTo(Path.Combine(environment.LocalRepository.TemplateFolder, "zh-Hans-CN.ldml")));

				// ensure that the template is used when the writing system is saved
				environment.LocalRepository.Set(chWs);
				environment.LocalRepository.Save();
				XElement ldmlElem = XElement.Load(environment.GetPathForLocalWSId("zh-Hans-CN"));
				Assert.That((string) ldmlElem.Elements("layout").Elements("orientation").Elements("lineOrder").First(), Is.EqualTo("top-to-bottom"));
			}
		}

		[Test]
		public void Save_UpdatesGlobalStore()
		{
			using (var environment = new TestEnvironment())
			using (var testFolder2 = new TemporaryFolder("LdmlInFolderWritingSystemRepositoryTests2"))
			{
				var ws = new WritingSystemDefinition("en-US");
				environment.LocalRepository.Set(ws);
				ws.RightToLeftScript = true;
				environment.LocalRepository.Save();
				Assert.IsTrue(File.Exists(environment.GetPathForLocalWSId("en-US")));
				Assert.IsTrue(File.Exists(environment.GetPathForGlobalWSId("en-US")));

				DateTime lastModified = File.GetLastWriteTime(environment.GetPathForGlobalWSId("en-US"));
				Thread.Sleep(1000);
				var localRepo2 = new TestLdmlInFolderWritingSystemRepository(testFolder2.Path, environment.GlobalRepository);
				ws = new WritingSystemDefinition("en-US");
				localRepo2.Set(ws);
				ws.RightToLeftScript = false;
				localRepo2.Save();
				Assert.Less(lastModified, File.GetLastWriteTime(environment.GetPathForGlobalWSId("en-US")));
			}
		}

		private static bool ContainsLanguageWithName(IEnumerable<WritingSystemDefinition> list, string name)
		{
			return list.Any(definition => definition.Language.Name == name);
		}

		class DummyWritingSystemProvider : IEnumerable<WritingSystemDefinition>
		{

			public IEnumerator<WritingSystemDefinition> GetEnumerator()
			{
				yield return new WritingSystemDefinition("en", "", "", "");
			}

			IEnumerator IEnumerable.GetEnumerator()
			{
				return GetEnumerator();
			}

		}

	}

	internal class DummyWritingSystemHandler
	{
		public static void OnMigration(IEnumerable<LdmlVersion0MigrationStrategy.MigrationInfo> migrationInfo)
		{
		}

		public static void OnLoadProblem(IEnumerable<WritingSystemRepositoryProblem> problems)
		{
		}

	}

}