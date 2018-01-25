using GQ.Client.Model;
using System.Xml.Serialization;

namespace GQ.Client.Model
{
	
	public interface IPage : IXmlSerializable, ITriggerContainer
	{
		int Id { get; }

		string PageType { get; }

		string Result { get; }

		string State { get; }

		Quest Parent { get; set; }

		void Start ();

		void End ();
	}

}
