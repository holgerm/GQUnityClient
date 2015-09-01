using System.IO;

namespace GQ.Util
{
	public class Files
	{

		static public void clearDirectory (string dirPath)
		{
			DirectoryInfo downloadedMessageInfo = new DirectoryInfo (dirPath);
			
			foreach (FileInfo file in downloadedMessageInfo.GetFiles()) {
				file.Delete (); 
			}

			foreach (DirectoryInfo dir in downloadedMessageInfo.GetDirectories()) {
				dir.Delete (true); 
			}
		}
	}
}
