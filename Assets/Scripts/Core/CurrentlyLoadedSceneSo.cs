using Gameplay;
using UnityEngine;

namespace Core
{
    [CreateAssetMenu(menuName = "ScriptableObjects/CurrentlyLoadedScene")]
    public class CurrentlyLoadedSceneSo : ScriptableObject
    {
        public LevelPropertiesSo loadedScene;
    }
}