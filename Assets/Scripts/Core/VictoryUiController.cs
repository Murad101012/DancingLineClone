using System;
using Interfaces;
using UnityEngine;
using UnityEngine.UI;

namespace Core
{
    public class VictoryUiController : MonoBehaviour, IVictory, IOnRestart
    {
        [Header("UI References")]
        [SerializeField] private GameObject victoryScreen;
        [SerializeField] private Button restartButton;
        
        private void OnEnable()
        {
            LevelRegistrySo.Instance.Register(this);
        }

        private void OnDisable()
        {
            LevelRegistrySo.Instance.Unregister(this);
        }

        public void OnVictory()
        {
            victoryScreen.SetActive(true);
        }

        public void OnLevelRestart()
        {
            victoryScreen.SetActive(false);
        }
    }
}