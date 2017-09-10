using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

namespace GQ.Client.Util {

	public class Directories {

		public static void DeleteDirCompletely(string path) {

			DirectoryInfo dir = new DirectoryInfo(path);

			foreach(FileInfo file in dir.GetFiles())
			{
				file.Delete();
			}

			foreach (DirectoryInfo subdir in dir.GetDirectories())
			{
				DeleteDirCompletely(subdir.FullName);
				subdir.Delete();
			}

			dir.Delete ();
		}

	}
}
