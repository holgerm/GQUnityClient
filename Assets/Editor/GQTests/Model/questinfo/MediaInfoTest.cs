using UnityEngine;
using UnityEditor;
using UnityEngine.TestTools;
using NUnit.Framework;
using System.Collections;
using GQ.Client.Model;
using System.Collections.Generic;
using Newtonsoft.Json;
using GQ.Client.Util;

namespace GQTests.Model {
	public class MediaInfoTest {

		[Test]
		public void LocalMediaRelativeDir() {
			// On iOS we have a certain Application.persistentDatapath:
			Device.SetPersistentDataPathMethod (() => { 
				return "/var/mobile/Containers/Data/Application/3F1D39E1-B44C-4C55-A709-DD2E254565CD/Documents"; 
			});

			// We have a file:
			string iOSAbsPathV1 = "/var/mobile/Containers/Data/Application/3F1D39E1-B44C-4C55-A709-DD2E254565CD/Documents/quests/10817/files/1_hollmann1.jpg";

			// And store the file:
			LocalMediaInfo lmi = new LocalMediaInfo("some.ulr", iOSAbsPathV1, "somefilename.txt", 123L, 1001L);

			// ASSERT that we get the absolute path when we retrieve the file:
			Assert.AreEqual(iOSAbsPathV1, lmi.absDir);

			// Then we udate the app and have a different persistent data path (on iOS):
			Device.SetPersistentDataPathMethod (() => { 
				return "/var/mobile/Containers/Data/Application/F32D581F-3C04-4C1C-A53B-2FCFA9490A1D/Documents"; 
			});

			// ASSERT that we get the new correct absolute path, when we access the file:
			Assert.AreEqual("/var/mobile/Containers/Data/Application/F32D581F-3C04-4C1C-A53B-2FCFA9490A1D/Documents/quests/10817/files/1_hollmann1.jpg", lmi.absDir);
		}


		[Test]
		public void TestJSONSerializationWithLocalMediaInfo() {
			// On iOS we have a certain Application.persistentDatapath:
			Device.SetPersistentDataPathMethod (() => { 
				return "/var/mobile/Containers/Data/Application/3F1D39E1-B44C-4C55-A709-DD2E254565CD/Documents"; 
			});

			// We have a file and store it into a LocalMediaInfo in a list of LMIs:
			string iOSAbsPathV1 = "/var/mobile/Containers/Data/Application/3F1D39E1-B44C-4C55-A709-DD2E254565CD/Documents/quests/10817/files/1_hollmann1.jpg";
			List<LocalMediaInfo> lmiList = new List<LocalMediaInfo> ();
			LocalMediaInfo lmi = new LocalMediaInfo("some.ulr", iOSAbsPathV1, "somefilename.txt", 123L, 1001L);
			lmiList.Add (lmi);

			// ASSERT that only the relative local path gets serialized:
			string json = JsonConvert.SerializeObject(lmiList, Newtonsoft.Json.Formatting.Indented);
			Debug.Log ("JSON: " + json);
			Assert.IsTrue (json.Contains ("/quests/10817/files/1_hollmann1.jpg")); 
			Assert.IsFalse (json.Contains (iOSAbsPathV1));

		}

	}
}