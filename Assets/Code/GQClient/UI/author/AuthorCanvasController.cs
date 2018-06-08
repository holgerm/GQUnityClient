using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using GQ.Client.Util;

public class AuthorCanvasController : MonoBehaviour
{

	public Button LoginButton;
	Text LoginButtonText;

	public Text AccountInput;
	public Text PasswordInput;
	private InputField AccountEmail;
	private InputField Password;
	public Text StatusText;

	const string LOGIN_TEXT = "Login";
	const string LOGOUT_TEXT = "Logout";

	const string NOT_LOGGED_IN_TEXT = "Sie sind <b>nicht</b> eingeloggt";
	const string LOGGED_IN_TEXTFORMAT = "Sie sind eingeloggt mit Autoren-Email\n<b>{0}</b>";


	// Use this for initialization
	void Start ()
	{
		AccountEmail = AccountInput.transform.Find ("InputField").GetComponent<InputField> ();
		Password = PasswordInput.transform.Find ("InputField").GetComponent<InputField> ();
		LoginButtonText = LoginButton.transform.Find ("Text").GetComponent<Text> ();
		checkLoginButtonText ();
		checkStatus ();
		checkInput (AccountEmail.text, Password.text); 
		Debug.Log ("AUTHOR CANVAS START() FInished. is activeSelf?: " + gameObject.activeSelf +
		"  is active in hierarchy: " + gameObject.activeInHierarchy);
		GameObject[] rootGOs = UnityEngine.SceneManagement.SceneManager.GetActiveScene ().GetRootGameObjects ();
		foreach (GameObject rootGo in rootGOs) {
			Canvas canv = rootGo.GetComponent<Canvas> ();
			if (canv != null) {
				Debug.Log ("Canvas " + canv.name + " on sorting order " + canv.sortingOrder + " root?: " + canv.isRootCanvas + " active&enabled: " + canv.isActiveAndEnabled);
			}
		}
	}

	public void EmailChanged (string newMail)
	{
		checkInput (newMail, Password.text);
	}


	public void PasswordChanged (string newPassword)
	{
		checkInput (AccountEmail.text, newPassword);
	}

	void checkInput (string mail, string passwd)
	{
		LoginButton.interactable = (
			(
				Base.Instance.LoggedInAs != null && 
				Base.Instance.LoggedInAs != ""
			) || 
			(
			    mail.Contains ("@") &&
			    mail.Length >= 6 &&
			    passwd.Length >= 3
			)
		);
	}

	/// <summary>
	/// Depending on state of the logged-in-state we set the status text and show or hide the input fields.
	/// </summary>
	void checkStatus ()
	{
		if (Base.Instance.LoggedInAs == null) {
			StatusText.text = NOT_LOGGED_IN_TEXT;
			AccountInput.gameObject.SetActive (true);
			PasswordInput.gameObject.SetActive (true);
		} else {
			StatusText.text = string.Format (LOGGED_IN_TEXTFORMAT, Base.Instance.LoggedInAs);
			AccountInput.gameObject.SetActive (false);
			PasswordInput.gameObject.SetActive (false);
			LoginButton.interactable = true;
		}
	}

	void checkLoginButtonText ()
	{
		LoginButtonText.text = 
			Base.Instance.LoggedInAs == null ? 
			LOGIN_TEXT : 
			LOGOUT_TEXT;
	}

	bool tryToLogin (string email, string password)
	{
		// TODO ask server for permissions ...
		Base.Instance.LoggedInAs = email;
		return true;
	}

	public void LoginPressed ()
	{
		switch (LoginButtonText.text) {
		case LOGIN_TEXT:
			if (tryToLogin (AccountEmail.text, Password.text)) {
				AccountEmail.text = "";
				Password.text = "";
			}
			checkStatus ();
			checkLoginButtonText ();
			break;
		case LOGOUT_TEXT:
			Base.Instance.LoggedInAs = null;
			checkStatus ();
			checkLoginButtonText ();
			break;
		}
		Debug.Log ("After Log Button Pressed, LoggedInAs is: " + (Base.Instance.LoggedInAs == null ? "[null]" : Base.Instance.LoggedInAs));
	}

}
