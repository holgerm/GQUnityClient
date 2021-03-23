using Code.GQClient.Util;
using NUnit.Framework;

namespace GQTests.Util
{

	public class Matrix_Rotation_90_Test
	{

		[Test]
		public void Rotate_90_0_0 ()
		{
			// Arrange:
			int[] original = new int[] { };

			// Act:
			int[] rotated = original.Rotate90 (0, 0);

			// Assert:
			Assert.AreEqual (original, rotated);
		}

		[Test]
		public void Rotate_90_1_1 ()
		{
			// Arrange:
			int[] original = new int[] { 
				1
			};

			// Act:
			int[] rotated = original.Rotate90 (1, 1);

			// Assert:
			Assert.AreEqual (original, rotated);
		}

		[Test]
		public void Rotate_90_3_2 ()
		{
			// Arrange:
			int[] original = new int[] { 
				1, 2, 3,
				4, 5, 6
			};

			// Act:
			int[] rotated = original.Rotate90 (2, 3);

			// Assert:
			int[] expected = new int[] {
				4, 1,
				5, 2,
				6, 3
			};
			Assert.AreEqual (expected, rotated);
		}

		[Test]
		public void Rotate_90_5_4 ()
		{
			// Arrange:
			int[] original = new int[] { 
				1,  2,  3,  4,  5,
				6,  7,  8,  9, 10,
				11, 12, 13, 14, 15,
				16, 17, 18, 19, 20
			};

			// Act:
			int[] rotated = original.Rotate90 (4, 5);

			// Assert:
			int[] expected = new int[] {
				16, 11, 6, 1,
				17, 12, 7, 2,
				18, 13, 8, 3,
				19, 14, 9, 4,
				20, 15, 10, 5
			};
			Assert.AreEqual (expected, rotated);
		}
	}
}