using UnityEngine;
using System.Collections;
using UnityEditor;
using UnityEngine.UI;

namespace QM.NFC
{

	[CustomPropertyDrawer (typeof(NFCReadTextPropertyAttribute))]
	public class NFC_AutoSetReaderField : PropertyDrawer
	{

		public override void OnGUI (Rect position, SerializedProperty property, GUIContent label)
		{
			NFC_ReaderUI myComponent = (NFC_ReaderUI)property.serializedObject.targetObject;
			Text[] textElements = myComponent.gameObject.GetComponentsInChildren<Text> (true);
			if (textElements.Length == 1 && myComponent.NFCPayloadText == null) {
				myComponent.NFCPayloadText = textElements [0];
			}

			EditorGUI.PropertyField (position, property, label);
			property.serializedObject.ApplyModifiedProperties ();
		}

	}

}
