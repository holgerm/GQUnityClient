using UnityEngine;
using System.Collections;
using System.IO;

namespace MediaKit.Processor
{
	class OGGStream
	{
		public readonly bool Preload;
		public readonly string Path;
		public FileStream InFile;
		public MemoryStream InMem;
		public bool Initializing;
		public bool Ready;
		public string Error;
		public int RefCount;
		
		public OGGStream(string path, bool preload)
		{
			Preload = preload;
			Path = path;
			Initializing = true;
		}
		
	}

}