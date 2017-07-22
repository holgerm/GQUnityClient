using UnityEngine;
using System.Collections;
using System.Xml.Serialization;
using System.Xml;
using System.Collections.Generic;

namespace GQ.Client.Model
{

	public interface IExpression : IXmlSerializable
	{

		Value Evaluate ();

	}

}
