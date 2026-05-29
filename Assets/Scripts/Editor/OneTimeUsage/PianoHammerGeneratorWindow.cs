using System.IO;
using UnityEditor;
using UnityEngine;

namespace Core
{
    public class ProjectLineCounter : EditorWindow
    {
        // Path relative to the project folder root
        private string _scriptsFolderPath = "Assets/Scripts"; 

        [MenuItem("Tools/Project Metrics/Line Counter")]
        public static void ShowWindow()
        {
            ProjectLineCounter window = GetWindow<ProjectLineCounter>("Code Line Counter");
            window.minSize = new Vector2(400, 200);
        }

        private void OnGUI()
        {
            GUILayout.Label("Project Code Metrics Utility", EditorStyles.boldLabel);
            EditorGUILayout.Space(10);

            EditorGUILayout.HelpBox("Specify your dedicated scripts directory target below. The engine will scan all matching sub-directories recursively.", MessageType.Info);
            EditorGUILayout.Space(5);

            // Path configuration slot
            _scriptsFolderPath = EditorGUILayout.TextField("Scripts Target Folder", _scriptsFolderPath);

            EditorGUILayout.Space(15);

            GUI.backgroundColor = new Color(0.2f, 0.6f, 0.9f); // Clean Blue Accent
            if (GUILayout.Button("Calculate Lines of Code", GUILayout.Height(40)))
            {
                CountProjectLines();
            }
            GUI.backgroundColor = Color.white;
        }

        private void CountProjectLines()
        {
            // Verify path health
            if (!Directory.Exists(_scriptsFolderPath))
            {
                Debug.LogError($"<color=red><b>Scan Failed:</b></color> Directory path '{_scriptsFolderPath}' does not exist inside your project structures. Double check spelling settings.");
                return;
            }

            // Grab all C# files recursively across all sub-folders
            string[] filePaths = Directory.GetFiles(_scriptsFolderPath, "*.cs", SearchOption.AllDirectories);
            
            int totalFilesFound = filePaths.Length;
            long rawTotalLines = 0;
            long pureCodeLines = 0;

            for (int i = 0; i < totalFilesFound; i++)
            {
                string filePath = filePaths[i];
                string[] lines = File.ReadAllLines(filePath);
                
                rawTotalLines += lines.Length;

                for (int j = 0; j < lines.Length; j++)
                {
                    string trimmedLine = lines[j].Trim();

                    // Filter out whitespace entries or documentation/comment marker rows
                    if (string.IsNullOrEmpty(trimmedLine) || 
                        trimmedLine.StartsWith("//") || 
                        trimmedLine.StartsWith("/*") || 
                        trimmedLine.StartsWith("*"))
                    {
                        continue;
                    }

                    pureCodeLines++;
                }
            }

            // Print highly detailed metric analysis straight back to the console window
            Debug.Log($"<color=cyan><b>=== PROJECT METRICS ANALYSIS COMPLETED ===</b></color>\n" +
                      $"• <b>Total C# Scripts Scanned:</b> {totalFilesFound}\n" +
                      $"• <b>Raw Total Lines (Incl. Spaces/Brackets):</b> {rawTotalLines:N0}\n" +
                      $"• <b>Pure Logical Code Lines (Excl. Comments/Empty):</b> <color=green><b>{pureCodeLines:N0}</b></color>");
        }
    }
}