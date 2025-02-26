using Code.GQClient.FileIO;
using Code.QM.Util;
using Newtonsoft.Json;

namespace Code.GQClient.Model.mgmt.quests
{

    /// <summary>
    /// Media Info about the local game media that is persisted in JSON file game-media.json in the quest folder.
    /// </summary>
    public class LocalMediaInfo
	{
		public string url;
		/// <summary>
		/// Only the relative part of the absolute dir path is persisted, since on iOS the application data folder changes between different app versions.
		/// </summary>
		public string dir;

		[JsonIgnore]
		public string absDir {
			get {
				if (dir == null) {
					return null;
				}

				return Files.CombinePath(Device.GetPersistentDatapath (), dir);
			}
			set {
				if (value == null) {
					dir = null;
					return;
				}

				if (value.StartsWith (Device.GetPersistentDatapath ())) {
					dir = value.Substring (Device.GetPersistentDatapath ().Length);
				} else {
					dir = value;
				}
			}
		}

		public string filename;
		public long size;
		public long time;

		public LocalMediaInfo (string url, string absDir, string filename, long size, long time)
		{
			this.url = url;
			this.absDir = absDir;
			this.filename = filename;
			this.size = size;
			this.time = time;
        }

        [JsonIgnore]
		public string LocalPath {
			get {
				return absDir + "/" + filename;
			}
		}

	}
}
