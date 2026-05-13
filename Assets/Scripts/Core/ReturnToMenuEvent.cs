using System;
using UnityEngine;
using UnityEngine.UI;

namespace Core
{
    /// <summary>
    /// Script to communicate with event (<see cref="OnReturnToMenu"/>) to <see cref="SceneLoader"/> for load Menu scene back
    /// </summary>
    /// <remarks>It's used in Ui prefabs on Level such as Defeat Ui/Victory Ui etc. where when player click on
    /// BackToMenuButton.prefab this script invoke and <see cref="ReturnToMenuHandler"/> send signal to <see cref="SceneLoader"/>
    /// for load the menu</remarks>
    public class ReturnToMenuEvent : MonoBehaviour
    {
        public static event Action OnReturnToMenu;
        private Button _returnToMenuButton;
        
        private void OnEnable()
        {
            _returnToMenuButton.onClick.AddListener(ReturnToMenu);
        }

        private void Awake()
        {
            _returnToMenuButton = GetComponent<Button>();
        }

        private void OnDisable()
        {
            _returnToMenuButton.onClick.RemoveListener(ReturnToMenu);
        }

        private void ReturnToMenu()
        {
            OnReturnToMenu?.Invoke();
        }
    }
}