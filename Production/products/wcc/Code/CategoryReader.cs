using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GQ.Client.Util;

namespace GQ.Client.Model
{

	public class CategoryReader
	{

		public static List<string> ReadCategoriesFromMetadata (MetaDataInfo[] metadata)
		{
			
			List<string> categories = new List<string> ();
			string netVal;
			foreach (MetaDataInfo md in metadata) {
				switch (md.Key) {
				case "category":
				case "category1":
					netVal = md.Value.StripQuotes ();
					if (netVal != "")
						categories.Insert (0, netVal);
					break;
				case "category2":
				case "category3":
					netVal = md.Value.StripQuotes ();
					if (netVal != "")
						categories.Add (netVal);
					break;
				}
			}
			return categories;
		}
	}

}