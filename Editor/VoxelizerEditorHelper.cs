using UnityEditor;
using UnityEngine;

namespace Editor
{
    public static class VoxelizerEditorHelper
    {
        public static GUIStyle WarningStyle = new(GUI.skin.label)
        {
            normal =
                { textColor = Color.yellow },
            fontStyle = FontStyle.Bold
        };


        public static GUIStyle ErrorStyle = new(GUI.skin.label)
        {
            normal =
                { textColor = Color.red },
            fontStyle = FontStyle.Bold
        };


        // helper method for drawing textField
        public static string DrawTextField(string label, string textContent, int fontSize)
        {
            var textLabelStyle = new GUIStyle(GUI.skin.label) { fontSize = fontSize };
            var textAreaStyle = new GUIStyle(GUI.skin.textField) { fontSize = fontSize };
            GUILayout.BeginHorizontal();
            GUILayout.Label(label, textLabelStyle, GUILayout.Width(EditorGUIUtility.currentViewWidth * 0.25f));
            textContent =
                EditorGUILayout.TextField(textContent, textAreaStyle,
                    GUILayout.Width(EditorGUIUtility.currentViewWidth * 0.75f));
            GUILayout.EndHorizontal();
            return textContent;
        }

        // override helper method for drawing textField
        public static string DrawTextField(GUIContent label, string textContent, int fontSize)
        {
            var textLabelStyle = new GUIStyle(GUI.skin.label) { fontSize = fontSize };
            var textAreaStyle = new GUIStyle(GUI.skin.textField)
                { fontSize = fontSize, fixedHeight = textLabelStyle.lineHeight + textLabelStyle.padding.vertical };
            GUILayout.BeginHorizontal();
            GUILayout.Label(label, textLabelStyle);
            GUILayout.FlexibleSpace(); // Pushes following elements to the right
            textContent =
                EditorGUILayout.TextField(textContent, textAreaStyle, GUILayout.ExpandWidth(true));
            GUILayout.EndHorizontal();
            return textContent;
        }

        public static bool DrawToggle(string label, bool toggle, int indent = 0)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Space(indent);
            GUILayout.Label(label);
            GUILayout.FlexibleSpace(); // Pushes following elements to the right
            var t = GUILayout.Toggle(toggle, GUIContent.none, GUILayout.Width(20));
            GUILayout.EndHorizontal();
            return t;
        }

        public static float DrawFloatSlider(string label, float currVal, float indent, float min = 0, float max = 100)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Space(indent);
            GUILayout.Label(label);
            var newInt = EditorGUILayout.Slider(currVal, min, max, GUILayout.ExpandWidth(true));
            GUILayout.EndHorizontal();

            return newInt;
        }

        public static int DrawIntSlider(string label, int currVal, int indent, int min = 0, int max = 100)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Space(indent);
            GUILayout.Label(label);
            var newInt = EditorGUILayout.IntSlider(currVal, min, max, GUILayout.ExpandWidth(true));
            GUILayout.EndHorizontal();

            return newInt;
        }
    }
}