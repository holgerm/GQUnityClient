using UnityEditor;
using UnityEngine;
using NUnit.Framework;
using System.Collections.Generic;
using GQ.Client.Net;

namespace GQTests.Net {

	public class SendQueueTest {

		[Test]
		public void SegmentFileIntoBlocks () {
			//Arrange
			List<byte> originalBytes = new List<byte>();
			List<byte[]> segmentedBytes = new List<byte[]>();

			for ( int i = 0; i < 10000; i++ ) {
				originalBytes.Add((byte)(i % 256));
			}

			//Act
			//Try to rename the GameObject
			segmentedBytes = SendHelper.prepareToSend(originalBytes);

			List<byte> composedAgainBytes = new List<byte>();

			foreach ( byte[] block in segmentedBytes ) {
				composedAgainBytes.AddRange(block);
			}

			//Assert
			//The object has a new name
			Assert.AreEqual(segmentedBytes.Count, Mathf.CeilToInt((float)originalBytes.Count / (float)SendHelper.BLOCKSIZE));
			Assert.AreEqual(originalBytes.Count, composedAgainBytes.Count);
			Assert.AreEqual(originalBytes, composedAgainBytes);
		}

		[Test]
		public void SendTextVars () {
			// TODO move this test to a class with tests with perfect connection

			// Arrange:
			// TODO create clientSideConnectionManager
			// TODO setup server mock with perfect connection
			// TODO prepare some text vars

			// Act:
			// TODO send text vars

			// Assert:
			// server has received the text vars as expected

			// check recombination of all parts
		}

	}
}