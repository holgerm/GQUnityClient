using UnityEngine;
using System.Collections;
using System.Text;
using System;

namespace GQ.Client.Model {

	/// <summary>
	/// Stores meta data about a quest, i.e. name, id, and some limited details about its content as well as usage data.
	/// </summary>
	public class QuestInfo {
		public int?  			id     				{ get; set; }

		public string   		name  				{ get; set; }

		public string 			featuredImagePath	{ get; set; }

		public int? 			typeID 				{ get; set; }

		public int? 			version 			{ get; set; }

		public string 			iconPath			{ get; set; }

		public long? 			lastUpdate 			{ get; set; }

		public HotspotInfo[] 	hotspots			{ get; set; }

		public MetaDataInfo[] 	metadata			{ get; set; }


		public string ToString () {
			StringBuilder sb = new StringBuilder();

			sb.AppendFormat("{0} (id: {1})\n", name, id);
			sb.AppendFormat("\t last update: {0}", lastUpdate);
			sb.AppendFormat("\t type id: {0}", typeID);
			sb.AppendFormat("\t version: {0}", version);
			sb.AppendFormat("\t icon path: {0}", iconPath);
			sb.AppendFormat("\t featured image path: {0}", featuredImagePath);
			sb.AppendFormat("\t with {0} hotspots.", hotspots == null ? 0 : hotspots.Length);
			sb.AppendFormat("\t and {0} metadata entries.", metadata == null ? 0 : metadata.Length);
	

			return sb.ToString();
		}

		public string GetMetadata (string key) {

			foreach ( MetaDataInfo md in metadata ) {
				if ( md.key.Equals(key) )
					return md.value;
			}

			return null;
		}
	}


	public struct HotspotInfo {

		public double? latitude { get; set; }

		public double? longitude { get; set; }
	}


	public struct MetaDataInfo {

		public string key { get; set; }

		public string value { get; set; }
	}




}