using UnityEditor;
using UnityEngine;
using NUnit.Framework;
using System.Collections.Generic;

public class SendQueueTest {

	[Test]
	public void EditorTest () {
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
	public void AddSegmentedArray () {
		// Arrange:
		List<byte> originalBytes = new List<byte>();
		List<byte[]> segmentedBytes = new List<byte[]>();

		for ( int i = 0; i < 10000; i++ ) {
			originalBytes.Add((byte)(i % 256));
		}

		// Act:
		// create new sendqueue
		// add all segments

		// Assert:
		// queue has start
		// queue has correct middle parts
		// queue has end

		// check recombination of all parts
	}

}
