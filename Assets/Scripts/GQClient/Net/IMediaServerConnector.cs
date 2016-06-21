using UnityEngine;
using System.Collections;

namespace GQ.Client.Net {
	/// <summary>
	/// Interface for Media Server. 
	/// </summary>
	public interface IMediaServerConnector {

		void send (SendQueueEntry message);

	}
}