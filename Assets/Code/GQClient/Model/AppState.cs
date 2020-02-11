namespace GQ.Model {

	public class AppState {

		private bool _connectedToServer;

		public bool ConnectedToServer {
			get {
				return _connectedToServer;
			}

			internal set {
				_connectedToServer = value;
			}
		}
	}
}
