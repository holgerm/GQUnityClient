using NUnit.Framework;

namespace GQTests
{
	
	public static class AssertExtension {

		public static void InRange(this NUnit.Framework.Assert assert, float lowerBound, float value, float upperBound) {
			if (lowerBound > value || value > upperBound) {
				throw new AssertionException (string.Format ("Expected: between {0} and {1}\nBut was: {2}", lowerBound, upperBound, value));
			}
		}
	}

}
