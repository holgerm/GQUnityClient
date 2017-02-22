using GQ.Util;


namespace GQ.Client.Net {
	public interface ISendQueue {

		void addTextMessage (string ip, string var, string text, int questId);

		void addFileMessage (string ip, string var, string filetype, byte[] bytes, int part, int questId);

		void addFileFinishMessage (string ip, string var, string filetype, int questId);

		void reconstructSendQueue ();
		// TODO rename to restore()

		int Count {
			get;
		}

		networkactions NetworkActionsObject {
			set;
		}

		void sendNext ();

		void removeMessage (int id);

		bool startConnectingToServer ();
		// TODO make private

	}
}