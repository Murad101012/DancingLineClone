using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.UIElements;

namespace Gameplay
{
    /// <summary>
    /// This ScriptableObject includes properties for level in both.. in level selection
    /// and while in the game
    /// </summary>
    /// <remarks>It's capabilities increased with LevelPropertiesEditor.cs and recommend to use
    /// "Fetch Data from current active scene" button under LevelProperties's inspector</remarks>
    [CreateAssetMenu(menuName = "ScriptableObjects/LevelProperties")]
    public class LevelPropertiesSo : ScriptableObject
    {
        //TODO: Change to Addressable type loading
        public string levelName;
        [SerializeField] public Sprite levelImage;
        public AudioClip levelSound;
        [HideInInspector] public StyleBackground styleBackgroundLevelImage;
        public AssetReference sceneLevel;
        
        //Required by MenuUiElementReference to disable/enable interactable of level change buttons
        public int totalLevels;
        public int levelIndex;

        //As soon as levelImage add from inspector, thi
        private void OnValidate()
        {
            if (levelImage != null)
            {
                styleBackgroundLevelImage = new StyleBackground(levelImage);
            }
        }
    }
}