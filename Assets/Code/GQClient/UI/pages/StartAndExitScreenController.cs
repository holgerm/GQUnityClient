using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GQ.Client.Model;
using UnityEngine.UI;
using GQ.Client.Util;
using GQ.Client.Err;
using System.Text.RegularExpressions;
using GQ.Client.Conf;

namespace GQ.Client.UI
{
	
	public class StartAndExitScreenController : PageController
	{

		#region Inspector Fields

		public RawImage image;

		#endregion

		#region Other Fields

		protected PageStartAndExitScreen saesPage;

		#endregion


		#region Runtime API

		//		// Use this for initialization
		//		public override void Start ()
		//		{
		//			base.Start ();
		//
		//			if (page == null)
		//				return;
		//
		//		}

		public override void Initialize ()
		{
			saesPage = (PageStartAndExitScreen)page;

			// show (or hide completely) image:
			GameObject imagePanel = image.transform.parent.gameObject;
			if (saesPage.ImageUrl == "") {
				imagePanel.SetActive (false);
				return;
			} else {
				imagePanel.SetActive (true);
				AbstractDownloader loader;
				if (saesPage.Parent.MediaStore.ContainsKey (saesPage.ImageUrl)) {
					MediaInfo mediaInfo;
					saesPage.Parent.MediaStore.TryGetValue (saesPage.ImageUrl, out mediaInfo);
					loader = new LocalFileLoader (mediaInfo.LocalPath);
				} else {
					loader = 
						new Downloader (
						url: saesPage.ImageUrl, 
						timeout: ConfigurationManager.Current.timeoutMS,
						maxIdleTime: ConfigurationManager.Current.maxIdleTimeMS
					);
					// TODO store the image locally ...
				}
				loader.OnSuccess += (AbstractDownloader d, DownloadEvent e) => {
					AspectRatioFitter fitter = image.GetComponent<AspectRatioFitter> ();
					fitter.aspectRatio = (float)d.Www.texture.width / (float)d.Www.texture.height;
					image.texture = d.Www.texture;
				};
				loader.Start ();
			}
		}

		#endregion


		#region implemented abstract members of PageController

		public override int NumberOfSpacesInContent ()
		{
			// we only have spaces between header and content as well as between content and footer:
			return 2;
		}

		#endregion


	}
}