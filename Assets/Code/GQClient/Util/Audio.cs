using System.Collections;
using System.Collections.Generic;
using Code.GQClient.Conf;
using Code.GQClient.Err;
using Code.GQClient.Model.mgmt.quests;
using Code.GQClient.Util.http;
using UnityEngine;
using UnityEngine.Networking;

namespace Code.GQClient.Util
{
    public class Audio
    {
        private static Dictionary<string, AudioSource> audioSources = new Dictionary<string, AudioSource>();

        public static void Clear()
        {
            List<AudioSource> deleteList = new List<AudioSource>();
            deleteList.AddRange(audioSources.Values);
            foreach (AudioSource audioSrc in deleteList)
            {
                audioSrc.Stop();
                if (audioSrc.clip != null)
                {
                    // in case the clip has not finished loading yet:
                    audioSrc.clip.UnloadAudioData();
                }

                audioSrc.clip = null;
                Base.Destroy(audioSrc);
            }

            audioSources = new Dictionary<string, AudioSource>();
        }

        /// <summary>
        /// Plays audio from media store.
        /// </summary>
        /// <returns>The length of the played audio in seconds.</returns>
        /// <param name="Url">URL.</param>
        /// <param name="loop">If set to <c>true</c> loop.</param>
        /// <param name="stopOtherAudio">If set to <c>true</c> stop other audio.</param>
        public static float PlayFromMediaStore(string Url, bool loop = false, bool stopOtherAudio = true)
        {
            if (QuestManager.Instance.MediaStore.TryGetValue(Url, out _))
            {
                return PlayFromFile(Url, loop, stopOtherAudio);
            }
            else
            {
                Log.SignalErrorToAuthor("Audio file referenced at {0} not locally stored.", Url);
                return 0f;
            }
        }

        /// <summary>
        /// Plays audio from given file.
        /// </summary>
        /// <returns>The length of the played audio in seconds.</returns>
        /// <param name="path">Path.</param>
        /// <param name="loop">If set to <c>true</c> loop.</param>
        /// <param name="stopOtherAudio">If set to <c>true</c> stop other audio.</param>
        public static float PlayFromFile(string path, bool loop, bool stopOtherAudio)
        {
            // lookup the dictionary of currently prepared audiosources
            if (audioSources.TryGetValue(path, out var audioSource))
            {
                _internalStartPlaying(audioSource, loop, stopOtherAudio);
                return audioSource.clip.length;
            }
            else
            {
                AbstractDownloader loader;
                string loadPath = null;
                if (QuestManager.Instance.MediaStore.ContainsKey(path))
                {
                    QuestManager.Instance.MediaStore.TryGetValue(path, out var mediaInfo);
                    loadPath = "file://" + mediaInfo.LocalPath;
                }
                else
                {
                    loadPath = path;
                }

                CoroutineStarter.Instance.StartCoroutine(GetAudioClip(loadPath, path, loop, stopOtherAudio));

                return 0f;
            }
        }

        /// <summary>
        /// Loads an Audio File from file or server and adds it to the cache of audio sources.
        /// </summary>
        /// <param name="path"></param>
        /// <param name="url">Used as key in the audiosources cache within the scene.</param>
        /// <param name="loop"></param>
        /// <param name="stopOtherAudio"></param>
        /// <returns></returns>
        static IEnumerator GetAudioClip(string path, string url, bool loop, bool stopOtherAudio)
        {
            using UnityWebRequest www = UnityWebRequestMultimedia.GetAudioClip(path, AudioType.UNKNOWN);
            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.ConnectionError)
            {
                Log.SignalErrorToAuthor($"Audio: Problem loading from {path} message: {www.error}");
            }
            else
            {
                var go = new GameObject("AudioSource for " + path);
                go.transform.SetParent(Base.Instance.transform);
                AudioSource audioSource = go.AddComponent<AudioSource>();
                audioSources[url] = audioSource;
                audioSource.clip = DownloadHandlerAudioClip.GetContent(www);
                _internalStartPlaying(audioSource, loop, stopOtherAudio);
            }
        }


        private static IEnumerator PlayAudioFileAsynch(string path, bool loop, bool stopOtherAudio)
        {
            GameObject go = new GameObject("AudioSource for " + path);
            go.transform.SetParent(Base.Instance.transform);
            AudioSource audioSource = go.AddComponent<AudioSource>();
            audioSources[path] = audioSource;
            // new AudioSource is stored in dictionary so it can be stopped already by Clear() etc.

            WWW audioWWW = new WWW(path);

            while (!audioWWW.isDone && audioSource != null)
            {
                // we wait until audio file is loaded and still audio not has been stopped in between:

                yield return null;
            }

            if (audioSource != null)
            {
                audioSource.clip = audioWWW.GetAudioClip(false, true);
                _internalStartPlaying(audioSource, loop, stopOtherAudio);
            }

            audioWWW.Dispose();
        }

        static void _internalStartPlaying(AudioSource audioSource, bool loop, bool stopOtherAudio)
        {
            Debug.Log("Audio: _internalStartPlaying begun ...");
            audioSource.loop = loop;
            if (stopOtherAudio)
                foreach (AudioSource audioSrc in audioSources.Values)
                {
                    if (!audioSrc.Equals(audioSource))
                    {
                        audioSrc.Stop();
                    }
                }

            audioSource.Play();
        }

        public static void StopAllAudio()
        {
            foreach (AudioSource audioSrc in audioSources.Values)
            {
                audioSrc.Stop();
            }
        }
    }
}