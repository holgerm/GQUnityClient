using UnityEngine;
using UnityEngine.UI;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Serialization;
using System.Runtime.Serialization;
using System;
using System.Linq;
using System.Text;
using GQ.Geo;
using GQ.Client.Util;
using GQ.Client.Model;

namespace GQ.Client.Model
{
	
	public interface IPage : IXmlSerializable, ITriggerContainer
	{
		int Id { get; }

		string Result { get; }

		string State { get; }

		Quest Parent { get; set; }

		void Start ();

		void End ();
	}

}
