using Code.GQClient.Model.mgmt.quests;
using Code.GQClient.Model.pages;
using UnityEngine;

namespace Code.GQClient.UI.pages.interactiveSphericalImage
{

	public class InteractionCtrl : MonoBehaviour
	{

		#region static stuff

		protected static readonly string PREFAB = "Interaction";

		protected Interaction myModel;

		public static InteractionCtrl Create (GameObject root, Interaction model)
		{
			// Create the game object for this controller:
			var go = PrefabController.Create ("prefabs", PREFAB, root);
			go.name = PREFAB + " (" + model.Id + ")";

			// initialize the Icon:
			QuestManager.Instance.LoadImageToTexture(
				model.Icon,
				texture =>
				{
					Sprite iconSprite = Sprite.Create(
						texture,
						new Rect(0, 0, texture.width, texture.height),
						new Vector2(0.5f, 0.5f));
					go.GetComponent<SpriteRenderer>().sprite = iconSprite;
				});
			
			// TODO set position and rotation etc. according to azimuth and altitude.
			
			var interactionCtrl = go.GetComponent<InteractionCtrl> ();
			go.SetActive(true);

			interactionCtrl.myModel = model;
			
			return interactionCtrl;
		}

		#endregion
		
		void OnMouseDown()
		{
			myModel.Tapped();
		}
	}
}