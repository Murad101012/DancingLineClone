using UnityEditor;
using UnityEngine;
using System.IO;

namespace Editor
{
    public class MeshExporter
    {
        [MenuItem("Assets/Export Selected Mesh Only", false, 10)]
        public static void ExportMesh()
        {
            // Get the selected object in the Project view
            GameObject selectedAsset = Selection.activeObject as GameObject;
            if (selectedAsset == null)
            {
                Debug.LogError("Please select an imported 3D model asset first.");
                return;
            }

            // Find the system path of the selected asset file
            string assetPath = AssetDatabase.GetAssetPath(Selection.activeObject);
            if (string.IsNullOrEmpty(assetPath))
            {
                Debug.LogError("Could not resolve the asset file path.");
                return;
            }

            MeshFilter meshFilter = selectedAsset.GetComponentInChildren<MeshFilter>();
            if (meshFilter == null || meshFilter.sharedMesh == null)
            {
                Debug.LogError("No MeshFilter found on the selected asset.");
                return;
            }

            // Get the directory containing the FBX file
            string targetDirectory = Path.GetDirectoryName(assetPath);
        
            // Generate the new target path inside that same directory
            string newMeshPath = Path.Combine(targetDirectory, $"{selectedAsset.name}_PureMesh.mesh");

            // Create a clean copy of the mesh to isolate it from the FBX file structure
            Mesh meshToSave = Object.Instantiate(meshFilter.sharedMesh);
        
            // Save it directly as a native Unity binary .mesh asset
            AssetDatabase.CreateAsset(meshToSave, newMeshPath);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh(); // Force Unity to update the folder view instantly
        
            Debug.Log($"Successfully exported pure mesh asset to: {newMeshPath}");
        }
    }
}