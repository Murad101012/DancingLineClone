using System;
using UnityEngine;

namespace Core
{
    [RequireComponent(typeof(LevelLoader))]
    public class ReturnToMenuHandler : MonoBehaviour
    {
        private LevelLoader _levelLoader;

        private void OnEnable()
        {
            ReturnToMenuEvent.OnReturnToMenu += OnReturnMenu;
        }

        private void Awake()
        {
            _levelLoader = GetComponent<LevelLoader>();
        }

        private void OnDisable()
        {
            ReturnToMenuEvent.OnReturnToMenu -= OnReturnMenu;
        }

        private void OnReturnMenu()
        {
            _levelLoader.ReturnToMenu();
        }
    }
}