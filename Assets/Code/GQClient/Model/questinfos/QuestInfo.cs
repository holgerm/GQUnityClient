using UnityEngine;
using System.Collections;
using System.Text;
using System;
using GQ.Client.Err;

namespace GQ.Client.Model {

	/// <summary>
	/// Stores meta data about a quest, i.e. name, id, and some limited details about its content as well as usage data.
	/// 
	/// A questInfo object has the following live cycle / states:
	/// 
	/// - The Quest exists only on Server and has not been downloaded yet or has just been delted. 
	///   (Initially if not predeployed)
	/// 	- Can be downloaded
	/// 	- Can NOT be started
	/// 	- Can NOT be updated
	/// 	- Can NOT be deleted
	/// - The Quest has been downloaded and exists locally as well as on server with same version. (After download)
	/// 	- Can NOT be downloaded
	/// 	- Can be started
	/// 	- Can NOT be updated
	/// 	- Can be deleted
	/// - The quest exists locally but has been updated on Server:
	/// 	- Can NOT be downloaded
	/// 	- Can be started
	/// 	- Can be updated
	/// 	- Can be deleted
	/// - The quest exists locally but has been removed from Server:
	/// 	- Can NOT be downloaded
	/// 	- Can be started
	/// 	- Can NOT be updated
	/// 	- Can be deleted but a warning should be shown
	/// The life cycle for quest loaded from server can be seen here: @ref QuestsFromServerLifeCycle
	/// 
	/// With predeployed quest:
	/// - The quest has been predeployed locally and there is no newer version on server:
	/// 	- Can NOT be downloaded
	/// 	- Can be started
	/// 	- Can NOT be updated
	/// 	- Can NOT be deleted
	/// - The quest has been predeployed locally but has been updated on Server:
	/// 	- Can NOT be downloaded
	/// 	- Can be started
	/// 	- Can be updated
	/// 	- Can NOT be deleted
	/// - The quest has been predeployed locally but updated locally to the newest server version:
	/// 	- Can NOT be downloaded
	/// 	- Can be started
	/// 	- Can be downgraded (set back to the older predeployed version)
	/// 	- Can NOT be deleted
	/// The life cycle for predeployed quest can be seen here: @ref QuestsPredeployedLifeCycle
	/// 
	/// We represent these states by four features with two or three values each:
	/// 
	/// - Downloadable (true, false)
	/// - Startable (true, false)
	/// - Updatable (true, false)
	/// - Deletable (Yes, YesWithWarning, No, Downgrade)


	/// </summary>
	public class QuestInfo 
	{
		public int  			Id     				{ get; set; }

		public string   		Name  				{ get; set; }

		public string 			FeaturedImagePath	{ get; set; }

		public int? 			TypeID 				{ get; set; }

		public string 			IconPath			{ get; set; }

		/// <summary>
		/// Server-side update timestamp.
		/// </summary>
		/// <value>The last update.</value>
		public long? 			LastUpdate 			{ get; set; }
		public long? LastUpdateOnServer {
			get {
				return LastUpdate;
			}
		}

		public HotspotInfo[] 	Hotspots			{ get; set; }

		public MetaDataInfo[] 	Metadata			{ get; set; }

		/// <summary>
		/// Client-side update timestamp;
		/// </summary>
		private long? lastUpdateOnDevice = null;

		public long? LastUpdatePersistentLocalStore = null;

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

		/// <summary>
		/// Determines whether this quest is locally available. This feature will be used in the UI in future versions.
		/// </summary>
		/// <returns><c>true</c> if this instance is locally available; otherwise, <c>false</c>.</returns>
		public bool IsLocallyStored () {
			return lastUpdateOnDevice != null || LastUpdatePersistentLocalStore != null;
		}

		/// <summary>
		/// Determines whether this quest is new. This feature will be used in the UI in future versions.
		/// </summary>
		/// <returns><c>true</c> if this instance is new; otherwise, <c>false</c>.</returns>
		public bool IsNew () {
			return playedTimes == 0;
		}

		public bool IsDownloadable () {
			return lastUpdateOnDevice == null && LastUpdateOnServer != null;
		}

		public bool IsUpdatable () {
			return (
			    // exists on both device and server:
			    lastUpdateOnDevice != null
				&& LastUpdateOnServer != null
				// server update is newer (bigger number):
				&& LastUpdateOnServer > lastUpdateOnDevice);
		}

		public enum Deletability { Delete, CanNotDelete, DeleleWithWarning, Downgrade }

		public Deletability GetDeletability () {
			if (!IsLocallyStored ())
				return Deletability.CanNotDelete;

			if (LastUpdatePersistentLocalStore != null &&
			   		lastUpdateOnDevice != null &&
			    	LastUpdatePersistentLocalStore > lastUpdateOnDevice) {
				return Deletability.Downgrade;
			}

			if (lastUpdateOnDevice != null && 
					LastUpdateOnServer != null &&
					lastUpdateOnDevice > LastUpdateOnServer) {
				return Deletability.DeleleWithWarning;
			}

			return Deletability.Delete;
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