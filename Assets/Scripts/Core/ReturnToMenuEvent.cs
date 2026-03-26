using System;
using UnityEngine;
using UnityEngine.UI;

namespace Core
{
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

        private void OnDestroy()
        {
            _returnToMenuButton.onClick.RemoveListener(ReturnToMenu);
        }

        private void ReturnToMenu()
        {
            OnReturnToMenu?.Invoke();
        }
    }
}