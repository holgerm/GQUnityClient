using UnityEngine;
using UnityEngine.Networking;

using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System;
using GQ.Client.Net;
using GQ.Client.Util;

namespace GQ.Client.Util {
	
	public interface IEnumerationWorker {

		void enumerate (IEnumerator enumerator);

	}


}