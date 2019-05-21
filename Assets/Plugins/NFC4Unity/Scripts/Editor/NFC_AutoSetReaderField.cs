using UnityEngine;
using System.Collections;
using UnityEditor;
using UnityEngine.UI;

namespace QM.NFC {

	[CustomPropertyDrawer(typeof(NFCReadTextPropertyAttribute))]
	public class NFC_AutoSetReaderField : PropertyDrawer {
		private static bool textForPayloadAutosettingDone = false;

		public override void OnGUI (Rect position, SerializedProperty property, GUIContent label) {
			NFC_Reader myComponent = (NFC_Reader)property.serializedObject.targetObject;
			Text[] textElements = myComponent.gameObject.GetComponentsInChildren<Text>(true);
			if ( !textForPayloadAutosettingDone && textElements.Length == 1 && myComponent.NFCPayloadText == null ) {
				myComponent.NFCPayloadText = textElements[0];
				textForPayloadAutosettingDone = true;
			}

			EditorGUI.PropertyField(position, property, label);
			property.serializedObject.ApplyModifiedProperties();
		}

	}

}
