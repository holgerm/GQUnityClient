using UnityEngine;
using UnityEditor;
using UnityEngine.TestTools;
using NUnit.Framework;
using System.Collections;
using GQ.Client.Model;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace GQTests.Model {
	public class QuestInfoFilters {

		QuestInfo qi_ABC, qi_CDE, qi_DEF, qi_GH;
		QuestInfo[] quests;

		[SetUp]
		public void CreateQuestInfos() {
			quests = JsonConvert.DeserializeObject<QuestInfo[]> (
				@"[
					{
				        ""id"": 1,
				        ""metadata"": [
				            {
				                ""key"": ""category"",
				                ""value"": ""A""
				            },
				            {
				                ""key"": ""category"",
				                ""value"": ""B""
				            },
				            {
				                ""key"": ""category"",
				                ""value"": ""C""
			            	}
				        ],
				        ""name"": ""ABC"",
				    },
					{
				        ""id"": 2,
				        ""metadata"": [
				            {
				                ""key"": ""category"",
				                ""value"": ""C""
				            },
				            {
				                ""key"": ""category"",
				                ""value"": ""D""
				            },
				            {
				                ""key"": ""category"",
				                ""value"": ""E""
			            	}
				        ],
				        ""name"": ""CDE"",
				    },
					{
				        ""id"": 3,
				        ""metadata"": [
				            {
				                ""key"": ""category"",
				                ""value"": ""D""
				            },
				            {
				                ""key"": ""category"",
				                ""value"": ""E""
				            },
				            {
				                ""key"": ""category"",
				                ""value"": ""F""
			            	}
				        ],
				        ""name"": ""DEF"",
				    },
					{
				        ""id"": 4,
				        ""metadata"": [
				            {
				                ""key"": ""category"",
				                ""value"": ""G""
				            },
				            {
				                ""key"": ""category"",
				                ""value"": ""H""
				            }
				        ],
				        ""name"": ""DEF"",
				    }
				]");
			
			qi_ABC = quests[0];
			qi_CDE = quests[1];
			qi_DEF = quests[2];
			qi_GH = quests [3];
		}

		[Test]
		public void AllQuests() {

			QuestInfoFilter filter_All = new QuestInfoFilter.All ();

			Assert.IsTrue (filter_All.Accept (qi_ABC));
			Assert.IsTrue (filter_All.Accept (qi_CDE));
			Assert.IsTrue (filter_All.Accept (qi_DEF));
			Assert.IsTrue (filter_All.Accept (qi_GH));

			Assert.AreEqual (qi_ABC.Categories, filter_All.AcceptedCategories(qi_ABC));
			Assert.AreEqual (qi_CDE.Categories, filter_All.AcceptedCategories(qi_CDE));
			Assert.AreEqual (qi_DEF.Categories, filter_All.AcceptedCategories(qi_DEF));
			Assert.AreEqual (qi_GH.Categories, filter_All.AcceptedCategories(qi_GH));

			Assert.AreEqual ("A", filter_All.CategoryToShow (qi_ABC));
			Assert.AreEqual ("C", filter_All.CategoryToShow (qi_CDE));
			Assert.AreEqual ("D", filter_All.CategoryToShow (qi_DEF));
			Assert.AreEqual ("G", filter_All.CategoryToShow (qi_GH));
		}

		[Test]
		public void CategoryFilter() {
			QuestInfoFilter filter_A = new QuestInfoFilter.CategoryFilter ("A");

			Assert.IsTrue (filter_A.Accept (qi_ABC));
			Assert.IsFalse (filter_A.Accept (qi_CDE));
			Assert.IsFalse (filter_A.Accept (qi_DEF));

			Assert.AreEqual (new List<string> { "A" }, filter_A.AcceptedCategories(qi_ABC));
			Assert.AreEqual (new List<string>(), filter_A.AcceptedCategories(qi_CDE));
			Assert.AreEqual (new List<string>(), filter_A.AcceptedCategories(qi_DEF));

			Assert.AreEqual ("A", filter_A.CategoryToShow (qi_ABC));
			Assert.AreEqual (QuestInfo.WITHOUT_CATEGORY_ID, filter_A.CategoryToShow (qi_CDE));
			Assert.AreEqual (QuestInfo.WITHOUT_CATEGORY_ID, filter_A.CategoryToShow (qi_DEF));
		
		
			QuestInfoFilter filter_C = new QuestInfoFilter.CategoryFilter ("C");

			Assert.IsTrue (filter_C.Accept (qi_ABC));
			Assert.IsTrue (filter_C.Accept (qi_CDE));
			Assert.IsFalse (filter_C.Accept (qi_DEF));

			Assert.AreEqual (new List<string> { "C" }, filter_C.AcceptedCategories(qi_ABC));
			Assert.AreEqual (new List<string> { "C" }, filter_C.AcceptedCategories(qi_CDE));
			Assert.AreEqual (new List<string>(), filter_C.AcceptedCategories(qi_DEF));

			Assert.AreEqual ("C", filter_C.CategoryToShow (qi_ABC));
			Assert.AreEqual ("C", filter_C.CategoryToShow (qi_CDE));
			Assert.AreEqual (QuestInfo.WITHOUT_CATEGORY_ID, filter_C.CategoryToShow (qi_DEF));


			QuestInfoFilter filter_CDE = new QuestInfoFilter.CategoryFilter ("C", "D", "E");

			Assert.IsTrue (filter_CDE.Accept (qi_ABC));
			Assert.IsTrue (filter_CDE.Accept (qi_CDE));
			Assert.IsTrue (filter_CDE.Accept (qi_DEF));

			Assert.AreEqual (new List<string> { "C" }, filter_CDE.AcceptedCategories(qi_ABC));
			Assert.AreEqual (new List<string> { "C", "D", "E" }, filter_CDE.AcceptedCategories(qi_CDE));
			Assert.AreEqual (new List<string> { "D", "E" }, filter_CDE.AcceptedCategories(qi_DEF));

			Assert.AreEqual ("C", filter_CDE.CategoryToShow (qi_ABC));
			Assert.AreEqual ("C", filter_CDE.CategoryToShow (qi_CDE));
			Assert.AreEqual ("D", filter_CDE.CategoryToShow (qi_DEF));
		}

		[Test]
		public void AndFilter() {
			QuestInfoFilter filter_A = new QuestInfoFilter.CategoryFilter ("A");
			QuestInfoFilter filter_C = new QuestInfoFilter.CategoryFilter ("C");
			QuestInfoFilter andFilter = new QuestInfoFilter.And (filter_A, filter_C);

			Assert.IsTrue (andFilter.Accept (qi_ABC));
			Assert.IsFalse (andFilter.Accept (qi_CDE));
			Assert.IsFalse (andFilter.Accept (qi_DEF));

			Assert.AreEqual (new List<string> { "A", "C" }, andFilter.AcceptedCategories(qi_ABC));
			Assert.AreEqual (new List<string> (), andFilter.AcceptedCategories(qi_CDE));
			Assert.AreEqual (new List<string> (), andFilter.AcceptedCategories(qi_DEF));

			Assert.AreEqual ("A", andFilter.CategoryToShow (qi_ABC));
			Assert.AreEqual (QuestInfo.WITHOUT_CATEGORY_ID, andFilter.CategoryToShow (qi_CDE));
			Assert.AreEqual (QuestInfo.WITHOUT_CATEGORY_ID, andFilter.CategoryToShow (qi_DEF));
		}

		[Test]
		public void OrFilter() {
			QuestInfoFilter filter_A = new QuestInfoFilter.CategoryFilter ("A");
			QuestInfoFilter filter_C = new QuestInfoFilter.CategoryFilter ("C");
			QuestInfoFilter filter_E = new QuestInfoFilter.CategoryFilter ("E");
			QuestInfoFilter orFilterAC = new QuestInfoFilter.Or (filter_A, filter_C);

			Assert.IsTrue (orFilterAC.Accept (qi_ABC));
			Assert.IsTrue (orFilterAC.Accept (qi_CDE));
			Assert.IsFalse (orFilterAC.Accept (qi_DEF));

			Assert.AreEqual (new List<string> { "A", "C" }, orFilterAC.AcceptedCategories(qi_ABC));
			Assert.AreEqual (new List<string> { "C" }, orFilterAC.AcceptedCategories(qi_CDE));
			Assert.AreEqual (new List<string> (), orFilterAC.AcceptedCategories(qi_DEF));

			QuestInfoFilter orFilterACE = new QuestInfoFilter.Or (orFilterAC, filter_E);

			Assert.IsTrue (orFilterACE.Accept (qi_ABC));
			Assert.IsTrue (orFilterACE.Accept (qi_CDE));
			Assert.IsTrue (orFilterACE.Accept (qi_DEF));
			Assert.IsFalse (orFilterACE.Accept (qi_GH));

			Assert.AreEqual (new List<string> { "A", "C" }, orFilterACE.AcceptedCategories(qi_ABC));
			Assert.AreEqual (new List<string> { "C", "E" }, orFilterACE.AcceptedCategories(qi_CDE));
			Assert.AreEqual (new List<string> { "E" }, orFilterACE.AcceptedCategories(qi_DEF));
			Assert.AreEqual (new List<string> (), orFilterACE.AcceptedCategories(qi_GH));

			Assert.AreEqual ("A", orFilterACE.CategoryToShow (qi_ABC));
			Assert.AreEqual ("C", orFilterACE.CategoryToShow (qi_CDE));
			Assert.AreEqual ("E", orFilterACE.CategoryToShow (qi_DEF));
			Assert.AreEqual (QuestInfo.WITHOUT_CATEGORY_ID, orFilterACE.CategoryToShow (qi_GH));
		}

	}
}