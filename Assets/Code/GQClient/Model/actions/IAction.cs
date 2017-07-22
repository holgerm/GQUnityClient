using UnityEngine;
using System.Collections;
using System.Xml.Serialization;

namespace GQ.Client.Model
{

	public interface IAction : IXmlSerializable, I_GQML, IParentedXml
	{
		void Execute ();

	}

}
