using Gameplay;
using UnityEngine;

namespace Core
{
    /// <summary>
    /// Source of truth that keeping information about currently level's information by <see cref="SceneLoader"/>
    /// </summary>
    [CreateAssetMenu(menuName = "ScriptableObjects/CurrentlyLoadedScene")]
    public class CurrentlyLoadedSceneSo : ScriptableObject
    {
        public LevelPropertiesSo loadedScene;
    }
}