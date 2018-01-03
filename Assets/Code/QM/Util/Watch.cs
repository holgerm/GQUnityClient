using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Diagnostics;

namespace QM.Util {
	
	public class WATCH {

		static Dictionary<string, Stopwatch> watches = new Dictionary<string, Stopwatch> ();

		public static Stopwatch Create(string name) {
			Stopwatch watch = new Stopwatch ();
			watches[name] = watch;
			return watch;
		}

		private static Stopwatch Get(string name) {
			Stopwatch watch;
			if (!watches.TryGetValue(name, out watch)) {
				return null;
			}
			return watch;
		}

		public static void StopAndShow(string name) {
			Stopwatch w = Get (name);
			if (w == null) {
				UnityEngine.Debug.Log (string.Format ("WATCH {0} not available.", name));
				return;
			}
			w.Stop ();
			UnityEngine.Debug.Log (string.Format ("WATCH {0} stopped after {1} ms.", name, w.ElapsedMilliseconds));
		}

		public static void Show(string name, string pointName) {
			Stopwatch w = Get (name);
			if (w == null) {
				UnityEngine.Debug.Log (string.Format ("WATCH {0} not available at {1}.", name, pointName));
				return;
			}
			long t = w.ElapsedMilliseconds;
			UnityEngine.Debug.Log (string.Format ("WATCH {0} at {1} took {2} ms.", name, pointName, t));
		}

		public static long Milliseconds(string name) {
			Stopwatch w = Get (name);
			if (w == null)
				return 0L;
			return w.ElapsedMilliseconds;
		}
	}
}
