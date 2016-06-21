using UnityEngine;
using System.Collections;

/// <summary>
/// Interface for Media Server. 
/// </summary>
public interface IMediaServerConnector {

	void send (SendQueueEntry message);

}
