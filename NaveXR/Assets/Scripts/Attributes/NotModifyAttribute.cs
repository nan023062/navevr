using System;
#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
#endif

namespace Nave.XR
{
    public class NotModifyAttribute :  UnityEngine.PropertyAttribute
    {
    }

#if UNITY_EDITOR

    [CustomPropertyDrawer(typeof(NotModifyAttribute))]
    public class NotModifyAttributeDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            string content = "[Undefined Type]";

            switch (property.propertyType)
            {
                case SerializedPropertyType.Integer:
                    content = property.longValue.ToString();
                    break;
                case SerializedPropertyType.Boolean:
                    content = property.boolValue.ToString();
                    break;
                case SerializedPropertyType.Float:
                    content = property.floatValue.ToString();
                    break;
                case SerializedPropertyType.String:
                    content = property.stringValue.ToString();
                    break;
                case SerializedPropertyType.Vector3:
                    content = string.Format("{0:N3},{1:N3},{2:N3}", property.vector3Value.x, 
                        property.vector3Value.y, property.vector3Value.z);
                    break;
                case SerializedPropertyType.Quaternion:
                    content = string.Format("{0:N2},{1:N2},{2:N2},{3:N2}", property.quaternionValue.x, 
                        property.quaternionValue.y, property.quaternionValue.z, property.quaternionValue.w);
                    break;
                case SerializedPropertyType.Enum:
                    content = property.enumNames[property.enumValueIndex];
                    break;
                default:
                    break;
            }
            EditorGUI.LabelField(position, label.text, content);
        }
    }


#endif
}
