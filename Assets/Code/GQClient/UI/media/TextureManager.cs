using System.Collections.Generic;
using UnityEngine;

namespace GQ.Client.UI {

	public class TextureManager {

		#region Singleton Instance 

		private static TextureManager _instance = null;

		public static TextureManager Instance {
			get {
				if (_instance == null) {
					_instance = new TextureManager ();
				}
				return _instance;
			}
		} 

		private TextureManager() {
			texDict = new Dictionary<string, Texture2D>();
		}

		#endregion


		#region Public API

		public Texture2D GetTexture(string textureID) {
			Texture2D tex = null;
			texDict.TryGetValue (textureID, out tex);
			return tex;
		}

		public bool Add(string textureID, Texture2D texture) {
			Texture2D t;
			if (texDict.TryGetValue(textureID, out t)) {
				return false;
			}
			else {
				texDict.Add (textureID, texture);
				return true;
			}
		}

		#endregion


		#region Internals

		private Dictionary<string, Texture2D> texDict;

		#endregion


		#region Static Helpers

		public static Color Blend(Color fg, Color bg) {
			Color result = new Color ();
			result.a = fg.a + bg.a * (1.0f - fg.a);
			if (result.a != 0.0f) {
				// otherwise we keep the initial zero values for r,g,b.
				result.r = (fg.r * fg.a + bg.r * bg.a * (1.0f - fg.a)) / result.a;
				result.g = (fg.g * fg.a + bg.g * bg.a * (1.0f - fg.a)) / result.a;
				result.b = (fg.b * fg.a + bg.b * bg.a * (1.0f - fg.a)) / result.a;
			}
			return result;
		}

		public static Color32 Blend(Color32 fg, Color32 bg) {
			Color32 result = new Color32 ();
			result.a = (byte) (fg.a + (float) bg.a * (1.0f - (float)fg.a));
			if (result.a != 0.0f) {
				// otherwise we keep the initial zero values for r,g,b.
				result.r = (byte) (((float)fg.r * (float)fg.a + (float)bg.r * (float)bg.a * (1.0f - (float)fg.a)) / (float)result.a);
				result.g = (byte) (((float)fg.g * (float)fg.a + (float)bg.g * (float)bg.a * (1.0f - (float)fg.a)) / (float)result.a);
				result.b = (byte) (((float)fg.b * (float)fg.a + (float)bg.b * (float)bg.a * (1.0f - (float)fg.a)) / (float)result.a);
			}
			return result;
		}

		#endregion
	}
}
