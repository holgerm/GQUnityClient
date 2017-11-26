using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GQ.Client.Err;
using System;

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
			texDict = new Dictionary<string, Texture>();
		}

		#endregion


		#region Public API

		public Texture GetTexture(string textureID) {
			Texture tex = null;
			texDict.TryGetValue (textureID, out tex);
			return tex;
		}

		public bool Add(string textureID, Texture texture) {
			Texture t;
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

		private Dictionary<string, Texture> texDict;

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

		#endregion
	}
}
