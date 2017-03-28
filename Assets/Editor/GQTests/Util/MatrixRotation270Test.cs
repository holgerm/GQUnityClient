using UnityEngine;
using UnityEditor;
using NUnit.Framework;
using GQ.Client.Util;

namespace GQTests.Util
{

	public class Matrix_Rotation_270_Test
	{

		[Test]
		public void Rotate_270_0_0 ()
		{
			// Arrange:
			int[] original = new int[] { };

			// Act:
			int[] rotated = original.Rotate270 (0, 0);

			// Assert:
			Assert.AreEqual (original, rotated);
		}

		[Test]
		public void Rotate_270_1_1 ()
		{
			// Arrange:
			int[] original = new int[] { 
				1
			};

			// Act:
			int[] rotated = original.Rotate270 (1, 1);

			// Assert:
			Assert.AreEqual (original, rotated);
		}

		[Test]
		public void Rotate_270_3_2 ()
		{
			// Arrange:
			int[] original = new int[] { 
				1, 2, 3,
				4, 5, 6
			};

			// Act:
			int[] rotated = original.Rotate270 (2, 3);

			// Assert:
			int[] expected = new int[] {
				3, 6,
				2, 5,
				1, 4
			};
			Assert.AreEqual (expected, rotated);
		}

		[Test]
		public void Rotate_270_5_4 ()
		{
			// Arrange:
			int[] original = new int[] { 
				1,  2,  3,  4,  5,
				6,  7,  8,  9, 10,
				11, 12, 13, 14, 15,
				16, 17, 18, 19, 20
			};

			// Act:
			int[] rotated = original.Rotate270 (4, 5);

			// Assert:
			int[] expected = new int[] {
				5, 10, 15, 20,
				4, 9, 14, 19,
				3, 8, 13, 18,
				2, 7, 12, 17,
				1, 6, 11, 16
			};
			Assert.AreEqual (expected, rotated);
		}
	}
}