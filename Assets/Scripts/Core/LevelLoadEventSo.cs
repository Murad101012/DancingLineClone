using System;
using UnityEngine;

namespace Core
{
    /// <summary>
    /// For breaking dependency between <see cref="Ui.Menu.MenuUiController"/> which is sending signal to load a level
    /// and <see cref="LevelLoader"/> which it's loading actual level, this ScriptableObject usefull.
    /// 
    /// Because, direct static event referencing from <see cref="Ui.Menu.MenuUiController"/> to <see cref="LevelLoader"/>
    /// makes impossible to null check in <see cref="LevelLoader"/> to if <see cref="Ui.Menu.MenuUiController"/> class available
    /// for listening to static event in <see cref="Ui.Menu.MenuUiController"/>. This scriptable object break this problem since we simple can check if reference to this
    /// Scriptable object is null (not assigned) or not (assigned)
    /// </summary>
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