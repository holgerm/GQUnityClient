using UnityEngine;
using UnityEditor;
using UnityEngine.TestTools;
using NUnit.Framework;
using System.Collections;
using System;
using Code.GQClient.Conf;
using Code.GQClient.UI.layout;
using GQTests;

namespace GQTests.Layout
{

	public class LayoutTests {

		[Test]
		public void Units2MM() {
			// prepare product json:
			ConfigurationManager.RetrieveProductJSONText = () => {
				return 
					@"{
							""id"": ""test"",
							""headerHeightUnits"": 60.0,
			 				""contentHeightUnits"": 750.0,
			 	 			""footerHeightUnits"": 75.0
					}";
			};

			DeviceDefinitions.Use (DeviceDefinitions.DeviceType.iPad4);

			// we have a totoal of 60 + 750 +75 = 885 units defined.
			// the device has a tital screen height of (2048p / 264p/inch) * 25,4inch/mm = 197mm.
			// we expect 1 unit to represent an absolute height of 197mm / 885 = 0.22259887.
			Assert.That(LayoutConfig.Units2MM(1f), Is.InRange(0.222f, 0.223f));
		}

		[Test]
		public void MM2Units() {
			// prepare product json:
			ConfigurationManager.RetrieveProductJSONText = () => {
				return 
					@"{
							""id"": ""test"",
							""headerHeightUnits"": 60.0,
			 				""contentHeightUnits"": 750.0,
			 	 			""footerHeightUnits"": 75.0
					}";
			};

			DeviceDefinitions.Use (DeviceDefinitions.DeviceType.iPad4);

			// we have a totoal of 60 + 750 +75 = 885 units defined.
			// the device has a tital screen height of (2048p / 264p/inch) * 25,4inch/mm = 197mm.
			// we expect 40mm to correspond to (885units / 197mm) * 40mm = 179,7units.
			Assert.That(LayoutConfig.MM2Units(40.0f), Is.InRange(179.6f, 179.8f));
		}

	}
}