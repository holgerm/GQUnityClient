using UnityEngine;
using UnityEditor;
using NUnit.Framework;
using UnityEditor.SceneManagement;
using GQ.Editor.Building;
using UnityEngine.SceneManagement;
using GQScripts;

namespace GQTests.Scripts {
	public class BaseTest {

		[Test]
		public void BaseGameObjectComplete () {
			// Arrange:
			Scene startScene = EditorSceneManager.OpenScene(ProductManager.START_SCENE);
			GameObject baseGO = GameObject.Find(Base.BASE);

			// Assert Base GO exists:
			Assert.NotNull(baseGO, "Base GameObject missing");

			// Assert Base GO has Base Component and is enabled:
			Base baseComp = baseGO.GetComponent<Base>();
			Assert.NotNull(baseComp, "Base Gameobject needs Base Component (Script)");
			Assert.That(baseComp.enabled, "Base Component must be enabled in Base GameObejct");

			// Assert Base GO has InitQuestManager Component and is enabled:
			InitQuestManager iqmComp = baseGO.GetComponent<InitQuestManager>();
			Assert.NotNull(iqmComp, "Base Gameobject needs InitQuestManager Component (Script)");
			Assert.That(iqmComp.enabled, "InitQuestManager Component must be enabled in Base GameObejct");
		}
	}
}
