using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace QM.Util
{
	public static class TypeExtension
	{

		static public Type[] ParameterTypes (this Type givenType)
		{
			string fullTypeName = givenType.FullName;
			Debug.Log ("ParameterTypes: " + fullTypeName);

			return null;
		}

	}
}