using UnityEngine;
using UnityEditor;
using NUnit.Framework;
using GQ.Editor.Util;
using GQTests;
using GQ.Client.Model;

namespace GQTests.Model {

	public abstract class DeserializationTest {

		protected string xml;
		protected QuestManager qm;

		[SetUp]
		public void Init () { 
		
			QuestManager.Reset();
			qm = QuestManager.Instance;
		}


	}
}
