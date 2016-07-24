using UnityEngine;
using UnityEngine.Events;

namespace QM.NFC
{


	[System.Serializable]
	public class NFCReadPayloadEvent : UnityEvent<string>
	{
	}

	[System.Serializable]
	public class NFCReadDetailsEvent : UnityEvent<NFC_Info>
	{
	}

}