//https://stackoverflow.com/questions/58984486/create-scriptable-object-with-constant-unique-id
using System;
using System.Security.Cryptography;
using UnityEditor;
using UnityEngine;

public class ScriptableObjectIdAttribute : PropertyAttribute { }

#if UNITY_EDITOR
[CustomPropertyDrawer(typeof(ScriptableObjectIdAttribute))]
public class ScriptableObjectIdDrawer : PropertyDrawer
{
    public override void OnGUI(Rect _position, SerializedProperty _property, GUIContent _label)
    {
        // Show ID
        {
            GUI.enabled = false;
            if (string.IsNullOrEmpty(_property.stringValue))
            {
                _property.stringValue = Guid.NewGuid().ToString();
            }
            EditorGUI.PropertyField(_position, _property, _label, true);
            GUI.enabled = true;
        }

        // Reset ID Button
        {
            Rect resetIDPosition = _position;
            resetIDPosition.y += 20.0f;
            resetIDPosition.height = 20.0f;

            if (GUI.Button(resetIDPosition, "Reset ID"))
            {
                _property.stringValue = Guid.NewGuid().ToString();
                Debug.Log("Setting new ID on object: " + _property.serializedObject.targetObject.name, _property.serializedObject.targetObject);
            }
        }
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        return 40.0f;
    }

    //Need to check for duplicates when copying a gameobject/component
    public static bool IsUnique<unityObjecType>(string ID, ObjectID _objectID)
    {
        Type unityObjectType = _objectID.UnityObject.GetType();
        return Resources.FindObjectsOfTypeAll(unityObjectType).Count(x => unityObjectType x.ID == ID) == 1;
    }
}
#endif
