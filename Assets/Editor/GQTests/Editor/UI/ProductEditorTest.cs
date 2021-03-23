using UnityEngine;
using UnityEditor;
using NUnit.Framework;
using System.IO;
using GQ.Editor.UI;

namespace GQTests.Editor.UI {

	public class ProductEditorTest {

		[Test]
		public void ImagesOK () {
			//Assert
			Assert.That(
				File.Exists(ProductEditor.WARN_ICON_PATH), 
				"Warn Icon missing. Should be at: " + ProductEditor.WARN_ICON_PATH);
			Assert.IsNotNull(
				(Texture)AssetDatabase.LoadAssetAtPath(ProductEditor.WARN_ICON_PATH, typeof(Texture)), 
				"Could not load Warn Icon from: " + ProductEditor.WARN_ICON_PATH);
		}

	}
}