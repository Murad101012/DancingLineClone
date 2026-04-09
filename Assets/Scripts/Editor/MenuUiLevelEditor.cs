using System.IO;
using System.Text.RegularExpressions;
using Ui.Menu;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

[CustomEditor(typeof(MenuUiLevelController))]
public class MenuUiLevelEditor : UnityEditor.Editor
{
    private MenuUiLevelController _menuUiLevelController;

    // We do the "Heavy Lifting" once when you click the object
    private void OnEnable()
    {
        _menuUiLevelController = (MenuUiLevelController)target;
    }
    

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        EditorGUILayout.Space();

        bool canRefresh = true;

        // Check Controller
        if (_menuUiLevelController == null)
        {
            EditorGUILayout.HelpBox("Controller not assigned!", MessageType.Error);
            canRefresh = false;
        }

        GUI.enabled = canRefresh; // Greys out the button if checks fail
        if (GUILayout.Button("Refresh Levels From List"))
        {
            RefreshLevels();
        }
        GUI.enabled = true;
    }

    private string GetUiDocumentAddress()
    {
        if (_menuUiLevelController == null)
        {
            return "0";
        }

        _menuUiLevelController.TryGetComponent(out UIDocument uiDocument);
        string relativePath;

        if (uiDocument != null)
        {
            // 1. Get the Unity Relative Path (e.g., "Assets/Menu.uxml")
            relativePath = AssetDatabase.GetAssetPath(uiDocument.visualTreeAsset);

            // 2. Get the Project Root (e.g., "/home/user/MyGame/")
            // Application.dataPath is ".../Assets", so we go one level up
            string projectRoot = Path.GetDirectoryName(Application.dataPath);

            // 3. Combine them to get the "Surgical" path for the SSD
            return Path.Combine(projectRoot, relativePath);
        }

        Debug.LogWarning($"{nameof(MenuUiLevelController)}: I couldn't find UIDocument under my parent");
        return "0";
    }

    private void RefreshLevels()
    {
        if (_menuUiLevelController.levelPropertiesSo == null) return;

        string fullPathOfUiDocument = GetUiDocumentAddress();
        if (fullPathOfUiDocument == "0") return;

        string originalXml = File.ReadAllText(fullPathOfUiDocument);

        string buttonsXml = "";
        for (int i = 0; i < _menuUiLevelController.levelPropertiesSo.Length; i++)
        {
            var level = _menuUiLevelController.levelPropertiesSo[i];
            string assetPath = AssetDatabase.GetAssetPath(level.levelImage);

            // If there is no image, we skip the style to avoid "Invalid Path" errors
            string styleString = "";
            if (!string.IsNullOrEmpty(assetPath))
            {
                // Added the missing '); at the end of the URL
                styleString = $"style=\"background-image: url('project://database/{assetPath}');\"";
            }

            buttonsXml += $"<ui:Button name=\"Btn_Background\" class=\"btn-level\" >\n                    <ui:Button name=\"{level.levelName}\" class=\"btn-level-image\" {styleString} />\n                </ui:Button>";

            //buttonsXml += $"\n <ui:Button name=\"{level.levelName}\" class=\"btn-level-image\" {styleString} />";
        }

        buttonsXml += "\n";

        File.WriteAllText(fullPathOfUiDocument, Regex.Replace(originalXml,
            @"(?<=<ui:VisualElement name=""Cont_Carousel""[^>]*>).*?(?=</ui:VisualElement>)", buttonsXml,
            RegexOptions.Singleline));

        AssetDatabase.Refresh();
    }
}