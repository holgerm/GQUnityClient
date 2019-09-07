using UnityEngine;
using UnityEngine.UI;
using GQ.Client.Util;
using GQ.Client.Conf;
using TMPro;

namespace GQ.Client.UI
{

    public class AuthorCanvasController : MonoBehaviour
    {

        public Button LoginButton;
        TextMeshProUGUI LoginButtonText;

        public TMP_InputField AccountInput;
        public TMP_InputField PasswordInput;
        public GameObject SettingsPanel;
        //public InputField AccountEmail;
        //public InputField Password;
        public TextMeshProUGUI StatusText;

        const string LOGIN_TEXT = "Login";
        const string LOGOUT_TEXT = "Logout";

        const string NOT_LOGGED_IN_TEXT = "Sie sind <b>nicht</b> eingeloggt";
        const string LOGGED_IN_TEXTFORMAT = "Sie sind eingeloggt mit Autoren-Email\n<b>{0}</b>";


        // Use this for initialization
        void Start()
        {
            //AccountEmail = AccountInput.transform.Find("InputField").GetComponent<InputField>();
            //Password = PasswordInput.transform.Find("InputField").GetComponent<InputField>();
            LoginButtonText = LoginButton.transform.Find("Text").GetComponent<TextMeshProUGUI>();
            checkLoginButtonText();
            checkStatus();
            checkInput(AccountInput.text, PasswordInput.text);
        }

        public void EmailChanged(string newMail)
        {
            checkInput(newMail, PasswordInput.text);
        }


        public void PasswordChanged(string newPassword)
        {
            checkInput(AccountInput.text, newPassword);
        }

        void checkInput(string mail, string passwd)
        {
            LoginButton.interactable = (
                (
                    Author.LoggedInAs != null &&
                    Author.LoggedInAs != ""
                ) ||
                (
                    mail.Contains("@") &&
                    mail.Length >= 6 &&
                    passwd.Length >= 3
                )
            );
        }

        /// <summary>
        /// Depending on state of the logged-in-state we set the status text and show or hide the input fields.
        /// </summary>
        void checkStatus()
        {
            if (Author.LoggedIn)
            {
                StatusText.text = string.Format(LOGGED_IN_TEXTFORMAT, Author.LoggedInAs);
                AccountInput.gameObject.SetActive(false);
                PasswordInput.gameObject.SetActive(false);
                SettingsPanel.gameObject.SetActive(true);
                LoginButton.interactable = true;
            }
            else
            {
                StatusText.text = NOT_LOGGED_IN_TEXT;
                AccountInput.gameObject.SetActive(true);
                PasswordInput.gameObject.SetActive(true);
                SettingsPanel.gameObject.SetActive(false);
            }
        }

        void checkLoginButtonText()
        {
            LoginButtonText.text =
                Author.LoggedIn ?
                LOGOUT_TEXT :
                LOGIN_TEXT;
        }

        bool tryToLogin(string email, string password)
        {
            // TODO ask server for permissions ...
            if (ConfigurationManager.Current.defineAuthorBackDoor)
            {
                if (email == ConfigurationManager.Current.acceptedAuthorEmail &&
                    password == ConfigurationManager.Current.acceptedAuthorPassword)
                {
                    Author.LoggedInAs = email;
                    //QuestInfoFilter.HiddenQuestsFilter.Instance.IsActive = true;
                    return true;
                }
            }

            // check for correct login server-side:
            return false;
        }

        public void LoginPressed()
        {
            if (Author.LoggedIn)
            {
                Author.LoggedInAs = null;
            }
            else
            {
                if (tryToLogin(AccountInput.text, PasswordInput.text))
                {
                    AccountInput.text = "";
                    PasswordInput.text = "";
                }
            }

            checkStatus();
            checkLoginButtonText();
        }
    }
}