using UnityEngine;
using System.Collections;
using System;

namespace GQ.Client.Util
{
	public class Values
	{

		public const double EPSILON = 0.0000000001d;

		public static bool NearlyEqual (double first, double second)
		{
			return Math.Abs (first - second) < EPSILON;
		}

		public static bool GreaterThan (double first, double second)
		{
			return first > second + EPSILON;
		}

	}
}
