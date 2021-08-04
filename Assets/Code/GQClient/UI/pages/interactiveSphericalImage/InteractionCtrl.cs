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
        protected Camera cam360;

        public static InteractionCtrl Create(GameObject root, Interaction model, Camera cam)
        {
            // Create the game object for this controller:
            var go = PrefabController.Create("prefabs", PREFAB, root);
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

            var interactionCtrl = go.GetComponent<InteractionCtrl>();
            go.SetActive(true);

            interactionCtrl.cam360 = cam;

            // set position and rotation etc. according to azimuth and altitude.
            interactionCtrl.myModel = model;
            interactionCtrl.transform.RotateAround(
                root.transform.position,
                Vector3.left,
                model.Altitude);
            interactionCtrl.transform.RotateAround(
                root.transform.position,
                Vector3.up,
                model.Azimuth + 90);  // this makes the image start at the center position,
                                      // since it originally starts at 1/4 of the horizon in the middle. 
                                      // I.e. we define NORTH at the 0 position.
            return interactionCtrl;
        }

        #endregion

        void OnMouseDown()
        {
            myModel.Tapped();
        }

        protected bool InFocus = false;

        void Update()
        {
            Vector3 viewPos = cam360.WorldToViewportPoint(transform.position);

            // NEWLY FOCUSSED:
            if (!InFocus && viewPos.z > 0f && viewPos.x > 0.2f && viewPos.x < 0.8f && viewPos.y > 0.2f &&
                viewPos.y < 0.8f)
            {
                Debug.Log($"Focussed: {myModel.Content} x: {viewPos.x} y: {viewPos.y}, z: {viewPos.z}");
                InFocus = true;
                myModel.Focussed();
            }

            // DEFOCUSSED AGAIN:
            if (InFocus && (viewPos.z < 0f || viewPos.x < 0f || viewPos.x > 1f || viewPos.y < 0f || viewPos.y > 1f))
            {
                Debug.Log($"DEFocussed: {myModel.Content} x: {viewPos.x} y: {viewPos.y}, z: {viewPos.z}");
                InFocus = false;
                myModel.DeFocussed();
            }
        }
    }
}