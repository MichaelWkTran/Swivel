//https://stackoverflow.com/questions/58984486/create-scriptable-object-with-constant-unique-id
using System;
using UnityEditor;
using UnityEngine;

public class ObjectIDAttribute : PropertyAttribute { }

#if UNITY_EDITOR
[CustomPropertyDrawer(typeof(ObjectIDAttribute))]
public class ScriptableObjectIdDrawer : PropertyDrawer
{
    public override void OnGUI(Rect _position, SerializedProperty _property, GUIContent _label)
    {
        float inputButtonRatio = 0.875f;
        
        //Show ID
        {
            Rect IDPosition = _position;
            IDPosition.width = _position.width * inputButtonRatio;

            if (string.IsNullOrEmpty(_property.stringValue))
            {
                _property.stringValue = Guid.NewGuid().ToString();
            }
            EditorGUI.PropertyField(IDPosition, _property, _label, true);
        }

        //Reset ID Button
        {
            Rect ButtonPosition = _position;
            ButtonPosition.x = _position.width * inputButtonRatio;
            ButtonPosition.width = _position.width * (1 - inputButtonRatio);

            if (GUI.Button(ButtonPosition, "Reset"))
            {
                _property.stringValue = Guid.NewGuid().ToString();
                Debug.Log("Setting new ID on object: " + _property.serializedObject.targetObject.name, _property.serializedObject.targetObject);
            }
        }
    }

    public static void ResetGuid(ref string _guid, UnityEngine.Object _targetObject)
    {
        _guid = Guid.NewGuid().ToString();

        if (_targetObject == null) Debug.Log("Setting new ID on object");
        else Debug.Log("Setting new ID on object: " + _targetObject.name, _targetObject);
    }
}
#endif
