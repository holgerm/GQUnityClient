using Code.GQClient.Err;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Code.QM.UI
{

    /// <summary>
    /// Add this component to a button to toggle the given GameObject on and off. 
    /// 
    /// Initial state depends on the activity setting of the gameobject whatToToggle.
    /// GameObject to toggle must be set in Inspector, otherwise the script will warn you and simply not work.
    /// </summary>
    [RequireComponent(typeof(Button)), RequireComponent(typeof(Image)), ExecuteInEditMode]
    public class ImageToggleButton : MonoBehaviour
    {
        public Button ToggleButton;
        public Image ToggleImage;

        public Sprite OnSprite;
        public Sprite OffSprite;

        public bool stateIsOn = true;

        public UnityEvent SwitchedOn;
        public UnityEvent SwitchedOff;

        private void Start()
        {
            ToggleButton = gameObject.GetComponentInChildren<Button>();

            if (ToggleButton == null)
            {
                Log.SignalErrorToDeveloper(
                    "{0} script could not be activated: missing a Button script on gameobject {1}.",
                    this.GetType().Name,
                    gameObject.name
                );
                return;
            }

            //ToggleButton.onClick.AddListener(Toggle); // BUGFIX: It had been called double: here and in the father object the catFolder.
            // TODO: we should refactor the whole CatFolder Prefab and its ingredients incl. this script class here.
            ToggleImage.sprite = stateIsOn ? OnSprite : OffSprite;
            ToggleButton.onClick.AddListener(Toggle);
        }

        public void Toggle()
        {
            stateIsOn = !stateIsOn;
            if (stateIsOn)
            {
                ToggleImage.sprite = OnSprite;
                SwitchedOn.Invoke();
            }
            else
            {
                ToggleImage.sprite = OffSprite;
                SwitchedOff.Invoke();
            }
        }

        private void OnDisable()
        {
            ToggleImage.enabled = false;
            ToggleButton.enabled = false;
        }

        private void OnEnable()
        {
            ToggleImage.enabled = true;
            ToggleButton.enabled = true;
        }
    }
}