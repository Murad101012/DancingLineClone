using Core;
using Player;
using UnityEditor;
using UnityEngine;

namespace Editor
{
    [CustomEditor(typeof(ObjectStatsSo))]
    public class PlayerStatsSoEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            // 'target' is a built-in variable in the Editor class. 
            // We just need to cast it to your specific type.
            ObjectStatsSo script = (ObjectStatsSo)target; 

            EditorGUILayout.Space();
            GUI.backgroundColor = Color.cyan;

            if (GUILayout.Button("Fetch Player properties from current active scene", GUILayout.Height(30)))
            {
                // Now 'script' is not null, so Undo.RecordObject will work!
                FetchPlayerData(script);
            }
        }

        private void FetchPlayerData(ObjectStatsSo script)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");

            if (player != null)
            {
                // Record the object so "Undo" (Ctrl+Z) works!
                Undo.RecordObject(script, "Fetch Player Data");

                script.firstLevelBeginPosition = player.transform.position;
                script.firstLevelBeginRotation = player.transform.rotation;

                // Mark the SO as 'dirty' so Unity knows it needs to be saved to the SSD
                EditorUtility.SetDirty(script);
                
                Debug.Log($"<color=green>Success:</color> Level data updated");
            }
            else
            {
                Debug.LogError("No GameObject with tag 'Player' found in the scene");
            }
        }
    }
}