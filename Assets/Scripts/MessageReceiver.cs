using UnityEngine;
using System.Collections;

/// <summary>
/// This class is used to handle messages sent from native code via UnitySendMessage().
/// </summary>
public class MessageReceiver : MonoBehaviour
{
    private static MessageReceiver messageReceiver;
	
	public string QRInfo;
	
    private AsyncOperation ao;

    void Awake ()
    {
        messageReceiver = this;
    }
 
    /// <summary>
    /// Calls the main Pause method
    /// </summary>
    /// <param name='msg'>
    /// Required by UnitySendMessage, otherwise it's unused by this method
    /// </param>
    public void Pause (string msg)
    {
        //GameManager.Pause ();
		Time.timeScale = 0;
    }
 
    /// <summary>
    /// Calls the main Pause method
    /// </summary>
    /// <param name='msg'>
    /// Required by UnitySendMessage, otherwise it's unused by this method
    /// </param>
    public void Unpause (string msg)
    {
        Time.timeScale = 1;
    }
	
	public void GetInfo(string info) 
	{
		QRInfo = info;
		Debug.Log("i am coming!!!!!"+info);
	}
 
    /// <summary>
    /// Returns an instance of MessageReceiver
    /// </summary>
    public static MessageReceiver Instance ()
    {
        return messageReceiver;
    }
}
