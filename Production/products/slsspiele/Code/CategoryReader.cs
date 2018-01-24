using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GQ.Client.Model;
using GQ.Client.Util;

namespace GQ.AppSpecific {

	public class CategoryReader {

		public static List<string> ReadCategoriesFromMetadata(MetaDataInfo[] metadata) {
			
			List<string> categories = new List<string> ();
			string netVal;
			foreach (MetaDataInfo md in metadata) {
				switch (md.Key) {
				case "Spieltyp1":
					netVal = md.Value.StripQuotes ();
					if (netVal != "")
						categories.Insert (0, netVal);
					break;
				case "Spieltyp2":
				case "Spieltyp3":
				case "Spieleranzahl":
				case "Dauer":
				case "Wetter":
				case "Orte":
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