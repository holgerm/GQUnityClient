using UnityEngine;
using System.Collections;
using UnityEditor.Animations;

namespace GQ.Client.Conf {

	public class LoadingCanvas {

		public static void Init (GameObject loadingCanvas) {

			Debug.Log("Initializing WCC LOading Canvas ...");

			UnityEngine.Animator anim = loadingCanvas.GetComponent<UnityEngine.Animator>();

			AnimatorController loadedCtrl = Resources.Load<AnimatorController>("animations/LoadingController");

			// ensure that state contains right animation motion:
			Motion loadedMotion = Resources.Load<Motion>("animations/LoadingAnimation");
			bool hasLink = hasMotionLink(loadedCtrl, loadedMotion);

			if ( !hasMotionLink(loadedCtrl, loadedMotion) )
				loadedCtrl.AddMotion(loadedMotion);

			// ensure that animator links to controller
			if ( anim.runtimeAnimatorController == null ) {
				anim.runtimeAnimatorController = loadedCtrl;
			}

		}

		private static bool hasMotionLink (AnimatorController ctrl, Motion searchedMotion) {
			AnimatorControllerLayer[] layers = ctrl.layers;
			if ( layers.Length == 0 )
				return false;

			ChildAnimatorState[] states = layers[0].stateMachine.states;

			foreach ( var stateStruct in states ) {
				if ( stateStruct.state.motion != null ) {
					if ( stateStruct.state.motion.Equals(searchedMotion) )
						return true; 
				}
				else {
					stateStruct.state.motion = searchedMotion;
					return true;
				}
			}

			return false;
		}
	}
}
