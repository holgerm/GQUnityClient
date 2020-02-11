namespace Code.GQClient.Lang {

	[System.Serializable]
	public class Translation {

		public string german;
		public string english;
		public string french;
		public string spanish;

		public Translation (string de, string en) {
			german = de;
			english = en;

		}

		public Translation (string de, string en, string fr, string es) {
			german = de;
			english = en;
			french = fr;
			spanish = es;
		}

	}
}
