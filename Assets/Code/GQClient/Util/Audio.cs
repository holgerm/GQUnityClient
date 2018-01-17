using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GQ.Client.Err;
using System;

namespace GQ.Client.Util
{

	public class Audio {

		private static Dictionary<string, AudioSource> audioSources = new Dictionary<string, AudioSource> ();

		public static void Clear() {
			List<AudioSource> deleteList = new List<AudioSource> ();
			deleteList.AddRange(audioSources.Values);
			foreach (AudioSource audioSrc in deleteList) {
				audioSrc.Stop ();
				if (audioSrc.clip != null) {
					// in case the clip has not finished loading yet:
					audioSrc.clip.UnloadAudioData ();
				}				
				audioSrc.clip = null;
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
			GameObject go = new GameObject ("AudioSource for " + path);
			go.transform.SetParent (Base.Instance.transform);
			AudioSource audioSource = go.AddComponent<AudioSource> ();
			audioSources [path] = audioSource;
			// new AudioSource is stored in dictionary so it can be stopped already by Clear() etc.

			WWW audioWWW = new WWW (path);

			while (!audioWWW.isDone && audioSource != null) {
				// we wait until audio file is loaded and still audio not has been stopped in between:

				yield return null;
			}

			if (audioSource != null) {
				audioSource.clip = audioWWW.GetAudioClip (false, true);
				_internalStartPlaying (audioSource, loop, stopOtherAudio);
			}

			audioWWW.Dispose ();
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
