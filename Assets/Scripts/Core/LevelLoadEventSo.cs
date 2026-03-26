using System;
using UnityEngine;

namespace Core
{
    /// <summary>
    /// Serves as an intermediary Event Channel for level loading operations.
    /// </summary>
    /// <remarks>
    /// This ScriptableObject facilitates **Decoupling** between UI triggers and the Scene Management logic.
    /// <para>Key Advantages:</para>
    /// <list type="bullet">
    /// <item>Prevents hard compile-time dependencies between UI and Core namespaces.</item>
    /// <item>Allows for runtime null-checks on the Asset reference, avoiding static event leaks.</item>
    /// <item>Enables testing UI or Level Loading in isolation without requiring both systems in the scene.</item>
    /// </list>
    /// </remarks>
    [CreateAssetMenu(fileName = "NewLevelLoadEventSo", menuName = "ScriptableObjects/LevelLoadEventSo")]
    public class LevelLoadEventSo : ScriptableObject
    {
        /// <summary>
        /// Invoke event when player choose to load player.
        /// </summary>
        public event Action<string> OnLevelNameLoad;

        public void RaiseOnLevelNameLoad(string levelName)
        {
            OnLevelNameLoad?.Invoke(levelName);
        }
    }
}