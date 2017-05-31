using UnityEngine;
using System.Collections;
using System.Xml.Serialization;
using System.Collections.Generic;
using System.Xml;

namespace GQ.Client.Model.XML
{

	public interface ICondition
	{

		bool IsFulfilled ();
	}

}