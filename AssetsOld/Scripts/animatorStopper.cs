using UnityEngine;
using System.Collections;

public class animatorStopper : MonoBehaviour {
public void stopAnimator(){


		GetComponent<Animator> ().enabled = false;

	}
}
