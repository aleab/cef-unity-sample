using Aleab.CefUnity.Structs;
using UnityEditor;
using UnityEngine;

namespace Aleab.CefUnity.UnityEditor
{
    [CustomPropertyDrawer(typeof(Size))]
    public class SizePropertyDrawer : PropertyDrawer
    {
        private SerializedProperty width;
        private SerializedProperty height;
        private string name;
        private bool cache = false;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (!this.cache)
            {
                // Get the name before it's gone.
                this.name = property.displayName;

                // Get the width and height values.
                property.Next(true);
                this.width = property.Copy();
                property.Next(true);
                this.height = property.Copy();

                this.cache = true;
            }

            Rect contentPosition = EditorGUI.PrefixLabel(position, new GUIContent(this.name));

            // Check if there is enough space to put the name on the same line (to save space).
            if (position.height > 16f)
            {
                position.height = 16f;
                EditorGUI.indentLevel += 1;
                contentPosition = EditorGUI.IndentedRect(position);
                contentPosition.y += 18f;
            }

            float half = contentPosition.width / 2;
            GUI.skin.label.padding = new RectOffset(3, 3, 6, 6);

            // Show the width and height.
            EditorGUIUtility.labelWidth = 14f;
            contentPosition.width *= 0.5f;
            EditorGUI.indentLevel = 0;

            // Begin/end property & change check make each field
            // behave correctly when multi-object editing.
            EditorGUI.BeginProperty(contentPosition, label, this.width);
            {
                EditorGUI.BeginChangeCheck();
                int newVal = EditorGUI.IntField(contentPosition, new GUIContent("W"), this.width.intValue);
                if (EditorGUI.EndChangeCheck())
                    this.width.intValue = newVal;
            }
            EditorGUI.EndProperty();

            contentPosition.x += half;

            EditorGUI.BeginProperty(contentPosition, label, this.height);
            {
                EditorGUI.BeginChangeCheck();
                int newVal = EditorGUI.IntField(contentPosition, new GUIContent("H"), this.height.intValue);
                if (EditorGUI.EndChangeCheck())
                    this.height.intValue = newVal;
            }
            EditorGUI.EndProperty();
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return Screen.width < 333 ? (16f + 18f) : 16f;
        }
    }
}