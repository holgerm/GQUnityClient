using UnityEngine;
using System.Collections;
using System.Text;
using System;
using GQ.Client.Err;

namespace GQ.Client.Model {

	/// <summary>
	/// Stores meta data about a quest, i.e. name, id, and some limited details about its content as well as usage data.
	/// </summary>
	public class QuestInfo 
	{
		public int?  			Id     				{ get; set; }

		public string   		Name  				{ get; set; }

		public string 			FeaturedImagePath	{ get; set; }

		public int? 			TypeID 				{ get; set; }

		public string 			IconPath			{ get; set; }

		public long? 			LastUpdate 			{ get; set; }

		public HotspotInfo[] 	Hotspots			{ get; set; }

		public MetaDataInfo[] 	Metadata			{ get; set; }

		private int? lastUpdateOnDevice = null;

		private int playedTimes = 0;

		public override string ToString () {
			StringBuilder sb = new StringBuilder();

			sb.AppendFormat("{0} (id: {1})\n", Name, Id);
			sb.AppendFormat("\t last update: {0}", LastUpdate);
			sb.AppendFormat("\t type id: {0}", TypeID);
			sb.AppendFormat("\t icon path: {0}", IconPath);
			sb.AppendFormat("\t featured image path: {0}", FeaturedImagePath);
			sb.AppendFormat("\t with {0} hotspots.", Hotspots == null ? 0 : Hotspots.Length);
			sb.AppendFormat("\t and {0} metadata entries.", Metadata == null ? 0 : Metadata.Length);
	
			return sb.ToString();
		}

		public string GetMetadata (string key) {

			foreach ( MetaDataInfo md in Metadata ) {
				if ( md.Key.Equals(key) )
					return md.Value;
			}

			return null;
		}

		public bool IsLocallyAvailable () {
			return lastUpdateOnDevice != null;
		}

		public bool IsNew () {
			return playedTimes == 0;
		}

		public bool IsDownloadable () {
			return lastUpdateOnDevice == null && LastUpdate != null;
		}

		public bool IsUpdatable () {
			return (
			    // exists on both device and server:
			    lastUpdateOnDevice != null
			    && LastUpdate != null
				// server update is newer (bigger number):
			    && LastUpdate > lastUpdateOnDevice);
		}

		public bool IsDeletable () {
			// TODO different for predeployed quests
			return lastUpdateOnDevice != null;
		}

		public bool WarnBeforeDeletion () {
			return IsDeletable() && LastUpdate == null;
		}
	}


	public struct HotspotInfo {

		public double? Latitude { get; set; }

		public double? Longitude { get; set; }
	}


	public struct MetaDataInfo {

		public string Key { get; set; }

		public string Value { get; set; }
	}

}