using UnityEngine;
using UnityEditor;
using UnityEngine.TestTools;
using NUnit.Framework;
using System.Collections;
using Newtonsoft.Json;
using GQ.Client.Conf;
using GQ.Client.Err;

namespace GQTests.Editor.JSON {

	public class JSONCOnfigTest {

		[Test]
		public void JSONColorConverterTest() {
			Config configOrigin = new Config ();

			Color c1 = new Color (2f, 3f, 4f, 5f);
			configOrigin.headerBgColor = c1;
				
			string json = JsonConvert.SerializeObject(configOrigin, Formatting.Indented);

			Config configRead = JsonConvert.DeserializeObject<Config> (json);

			Color r1 = configRead.headerBgColor;

			Assert.AreEqual (c1.r, r1.r);
			Assert.AreEqual (c1.g, r1.g);
			Assert.AreEqual (c1.b, r1.b);
			Assert.AreEqual (c1.a, r1.a);
		}

	}

}
