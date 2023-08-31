//using System;
//using System.Linq;
//using Unity.VisualScripting;
//using UnityEngine;
//using UnityEngine.UIElements;

//[Serializable] public struct ObjectID
//{
//    [SerializeField] UniqueID _id;
//    [SerializeField] UnityEngine.Object m_unityObject;
//    public UnityEngine.Object UnityObject { get { return m_unityObject; } }
//    public string ID { get { return _id.Value; } }

//    [ContextMenu("Force reset ID")]
//    void ResetId()
//    {
//        _id.Value = Guid.NewGuid().ToString();

//        Debug.Log("Setting new ID on object: " + m_unityObject.ToString(), m_unityObject);
//    }

//    //Need to check for duplicates when copying a gameobject/component
//    public static bool IsUnique<unityObjecType>(string ID, ObjectID _objectID)
//    {
//        Type unityObjectType = _objectID.UnityObject.GetType();
//        return Resources.FindObjectsOfTypeAll(unityObjectType).Count(x => unityObjectType x.ID == ID) == 1;
//    }

//    public void OnValidate(GameObject _gameObject)
//    {
//        //If scene is not valid, the gameobject is most likely not instantiated (ex. prefabs)
//        if (!_gameObject.scene.IsValid())
//        {
//            _id.Value = string.Empty;
//            return;
//        }

//        if (string.IsNullOrEmpty(ID) || !IsUnique(ID))
//        {
//            ResetId();
//        }
//    }

//    [Serializable] struct UniqueID { public string Value; }

//#if UNITY_EDITOR

//    [UnityEditor.CustomPropertyDrawer(typeof(UniqueID))]
//    private class UniqueIdDrawer : UnityEditor.PropertyDrawer
//    {
//        private const float buttonWidth = 120;
//        private const float padding = 2;

//        public override void OnGUI(Rect position, UnityEditor.SerializedProperty property, GUIContent label)
//        {
//            UnityEditor.EditorGUI.BeginProperty(position, label, property);

//            // Draw label
//            position = UnityEditor.EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);

//            GUI.enabled = false;
//            Rect valueRect = position;
//            valueRect.width -= buttonWidth + padding;
//            UnityEditor.SerializedProperty idProperty = property.FindPropertyRelative("Value");
//            UnityEditor.EditorGUI.PropertyField(valueRect, idProperty, GUIContent.none);

//            GUI.enabled = true;

//            Rect buttonRect = position;
//            buttonRect.x += position.width - buttonWidth;
//            buttonRect.width = buttonWidth;
//            if (GUI.Button(buttonRect, "Copy to clipboard"))
//            {
//                UnityEditor.EditorGUIUtility.systemCopyBuffer = idProperty.stringValue;
//            }

//            UnityEditor.EditorGUI.EndProperty();
//        }
//    }
//#endif
//}
