
namespace GQ.Client.Net {
	public interface ISendQueue {

		void addTextMessage (string ip, string var, string text, int questId);
	}
}