using UnityEngine;
using UnityEditor;
using NUnit.Framework;
using GQ.Client.Util;

namespace GQTests.Util
{

	public class Matrix_Rotation_180_Test
	{

		[Test]
		public void Rotate_180_0_0 ()
		{
			// Arrange:
			int[] original = new int[] { };

			// Act:
			int[] rotated = original.Rotate180 (0, 0);

			// Assert:
			Assert.AreEqual (original, rotated);
		}

		[Test]
		public void Rotate_180_1_1 ()
		{
			// Arrange:
			int[] original = new int[] { 
				1
			};

			// Act:
			int[] rotated = original.Rotate180 (1, 1);

			// Assert:
			Assert.AreEqual (original, rotated);
		}

		[Test]
		public void Rotate_180_3_2 ()
		{
			// Arrange:
			int[] original = new int[] { 
				1, 2, 3,
				4, 5, 6
			};

			// Act:
			int[] rotated = original.Rotate180 (2, 3);

			// Assert:
			int[] expected = new int[] {
				6, 5, 4,
				3, 2, 1
			};
			Assert.AreEqual (expected, rotated);
		}

		[Test]
		public void Rotate_180_5_4 ()
		{
			// Arrange:
			int[] original = new int[] { 
				1,  2,  3,  4,  5,
				6,  7,  8,  9, 10,
				11, 12, 13, 14, 15,
				16, 17, 18, 19, 20
			};

			// Act:
			int[] rotated = original.Rotate180 (4, 5);

			// Assert:
			int[] expected = new int[] {
				20, 19, 18, 17, 16,
				15, 14, 13, 12, 11, 
				10, 9, 8, 7, 6,
				5, 4, 3, 2, 1
			};
			Assert.AreEqual (expected, rotated);
		}
	}
}