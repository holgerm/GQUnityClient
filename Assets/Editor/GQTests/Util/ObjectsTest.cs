using Code.GQClient.Util;
using UnityEngine;
using NUnit.Framework;

namespace GQTests.Util {
	public class ObjectsTest {

		[Test]
		public void TestToString () {
			//Assert
			//The object has a new name
			Assert.AreEqual("New Game Object (UnityEngine.GameObject)", Objects.ToString(new GameObject()));
			Assert.AreEqual("42", Objects.ToString(42));
			Assert.AreEqual("text", Objects.ToString("text"));
			Assert.AreEqual("4.02", Objects.ToString(4.02f));
			Assert.AreEqual("4.02", Objects.ToString(4.02d));
			Assert.AreEqual("[null]", Objects.ToString(null));
		}
	}
}
