using UnityEngine;
using UnityEditor;
using NUnit.Framework;

namespace GQTests.Net {

	public class FileDownloadExperiment {

		[Test]
		public void LoadFileHeaders () {
			//Arrange
			var gameObject = new GameObject();

			//Act
			//Try to rename the GameObject
			var newGameObjectName = "My game object";
			gameObject.name = newGameObjectName;

			//Assert
			//The object has a new name
			Assert.AreEqual(newGameObjectName, gameObject.name);
		}
	}
}
