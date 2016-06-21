using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace GQ.Client.Net {

	public class SendQueueHelper {

		public const string MODE_VALUE = "value";
		public const string MODE_FILE_START = "file_start";
		public const string MODE_FILE_MID = "file_mid";
		public const string MODE_FILE_FINISH = "file_finish";


		/// heuristik über maximale länge einer nachricht an den server
		public const int BLOCKSIZE = 1300;

		public static List<byte[]> prepareToSend (List<byte> orginialBytes) {
			List<byte[]> sendbytes = new List<byte[]>();

			int size = BLOCKSIZE;

			for ( int i = 0; i < orginialBytes.Count; i += size ) {
				var list = new List<byte>();

				// höchtens bis zum ende gehen:
				if ( (i + size) > orginialBytes.Count ) {
					size = orginialBytes.Count - (i);
				}

				sendbytes.Add(orginialBytes.GetRange(i, size).ToArray());
			}

			return sendbytes;
		}

	}
}