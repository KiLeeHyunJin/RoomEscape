using System;
using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(EnumFlagsAttribute))]
public class EnumFlagsAttributeDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);

        EnumFlagsAttribute flagSettings = (EnumFlagsAttribute)attribute;
        Enum targetEnum = (Enum)fieldInfo.GetValue(property.serializedObject.targetObject);

        string propName = property.displayName;
        EditorGUI.BeginChangeCheck();
        Enum enumNew = EditorGUI.EnumFlagsField(position, propName, targetEnum);
        if (EditorGUI.EndChangeCheck())
        {
            property.intValue = (int)Convert.ChangeType(enumNew, targetEnum.GetType());
        }

        EditorGUI.EndProperty();
    }
}
