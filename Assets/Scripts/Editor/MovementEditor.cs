using System;
using UnityEngine;
using UnityEditor;
using Player; // Your namespace

namespace Editor
{
    [CustomEditor(typeof(Movement))]
    public class MovementEditor : UnityEditor.Editor
    {
        private Color _originalColor;

        private void OnEnable()
        {
            GUI.backgroundColor = _originalColor;
        }

        public override void OnInspectorGUI()
        {
            // Draw the default inspector first (so we see the fields)
            DrawDefaultInspector();

            Movement movement = (Movement)target;

            // Use serializedObject to find the property safely
            SerializedProperty statsProp = serializedObject.FindProperty("objectStatsSo");

            EditorGUILayout.Space(); // Add some breathing room

            if (statsProp.objectReferenceValue == null)
            {
                GUI.backgroundColor = Color.red;
                // Draw a professional HelpBox if the SO is missing
                EditorGUILayout.HelpBox(
                    "CRITICAL: PlayerStatsSo is missing! Please assign a Stats Asset.",
                    MessageType.Error);
            }
            GUI.backgroundColor = _originalColor;
        }
    }
}