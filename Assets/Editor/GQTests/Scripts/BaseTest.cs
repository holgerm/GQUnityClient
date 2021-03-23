using Code.GQClient.Util;
using UnityEngine;
using NUnit.Framework;

namespace GQTests.Scripts
{
	public class BaseTest
	{

		[Test]
		public void BaseGameObjectComplete ()
		{
			// Arrange:
			GameObject baseGO = GameObject.Find (Base.BASE);

			// Assert Base GO exists:
			Assert.NotNull (baseGO, "Base GameObject missing");

			// Assert Base GO has Base Component and is enabled:
			Base baseComp = baseGO.GetComponent<Base> ();
			Assert.NotNull (baseComp, "Base Gameobject needs Base Component (Script)");
			Assert.That (baseComp.enabled, "Base Component must be enabled in Base GameObejct");
		}
	}
}
