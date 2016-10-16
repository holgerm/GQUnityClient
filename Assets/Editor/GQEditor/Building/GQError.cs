using UnityEngine;
using System.Collections;

namespace GQ.Editor.Building {

	public class GQError {

		protected string _text;

		public string Text {
			get {
				return _text;
			}
			set {
				_text = value;
			}
		}


		public GQError (string text) {
			Text = text;
		}

		override public string ToString () {
			return Text;
		}

	}
}

