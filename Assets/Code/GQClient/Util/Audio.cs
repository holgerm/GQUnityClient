using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GQ.Client.Err;

namespace GQ.Client.Util
{

	public class Audio {

		private static Dictionary<string, AudioSource> audioSources = new Dictionary<string, AudioSource> ();

		public static void Clear() {
			List<AudioSource> deleteList = new List<AudioSource> ();
			deleteList.AddRange(audioSources.Values);
			foreach (AudioSource audioSrc in deleteList) {
				audioSrc.Stop ();
				Base.Destroy (audioSrc);
			}
			audioSources = new Dictionary<string, AudioSource> ();
		}

		public static void PlayFromFile(string path, bool loop, bool stopOtherAudio) {
			// lookup the dictionary of currently prepared audiosources
			AudioSource audioSource = null;
			if (audioSources.TryGetValue(path, out audioSource)) {
				_internalStartPlaying (audioSource, loop, stopOtherAudio);
				return;
			}
			else {
				// First load the audio file asynch and then play it:
				Base.Instance.StartCoroutine(PlayAudioFileAsynch(path, loop, stopOtherAudio));
			}
		}

		private static IEnumerator PlayAudioFileAsynch(string path, bool loop, bool stopOtherAudio) {
			WWW audioWWW = new WWW (path);

			while (!audioWWW.isDone) {
				yield return null;
			}

			GameObject go = new GameObject ("AudioSource for " + path);
			go.transform.SetParent (Base.Instance.transform);
			AudioSource audioSource = go.AddComponent<AudioSource> ();
			audioSource.clip = audioWWW.GetAudioClip (false, false);
			audioSources [path] = audioSource;
			_internalStartPlaying (audioSource, loop, stopOtherAudio);

			yield break;
		}

		static void _internalStartPlaying (AudioSource audioSource, bool loop, bool stopOtherAudio)
		{
			audioSource.loop = loop;
			if (stopOtherAudio)
				foreach (AudioSource audioSrc in audioSources.Values) {
					if (!audioSrc.Equals (audioSource)) {
						audioSrc.Stop ();
					}
				}
			audioSource.Play ();
		}
	}
}
