using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GQ.Client.Err;
using System;
using GQ.Client.Model;
using GQ.Client.Conf;

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
//				// First load the audio file asynch and then play it:
//				MediaInfo mediaInfo;
//				QuestManager.Instance.CurrentQuest.MediaStore.TryGetValue (path, out mediaInfo);

				// NEW:
				AbstractDownloader loader;
				if (QuestManager.Instance.CurrentQuest.MediaStore.ContainsKey (path)) {
					MediaInfo mediaInfo;
					QuestManager.Instance.CurrentQuest.MediaStore.TryGetValue (path, out mediaInfo);
					loader = new LocalFileLoader (mediaInfo.LocalPath);
				}
				else {
					loader = new Downloader (url: path, timeout: ConfigurationManager.Current.timeoutMS);
					// TODO store the image locally ...
				}
				loader.OnSuccess += (AbstractDownloader d, DownloadEvent e) =>  {
					GameObject go = new GameObject ("AudioSource for " + path);
					go.transform.SetParent (Base.Instance.transform);
					audioSource = go.AddComponent<AudioSource> ();
					audioSources [path] = audioSource;

					audioSource.clip = d.Www.GetAudioClip (false, true);
					_internalStartPlaying (audioSource, loop, stopOtherAudio);
					// Dispose www including it s Texture and take some logs for preformace surveillance:
					d.Www.Dispose ();
				};
				loader.Start ();
//
//
//				Base.Instance.StartCoroutine(PlayAudioFileAsynch(mediaInfo.LocalPath, loop, stopOtherAudio));
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
