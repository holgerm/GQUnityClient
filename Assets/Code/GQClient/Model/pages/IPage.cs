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

		bool CanStart ();

		void Start ();

		void End ();

		/// <summary>
		/// Called just before this page is left and another page type or the foyer will follow. You might use overriding implementations to unregister listeners etc.
		/// </summary>
		void CleanUp ();
	}

}
