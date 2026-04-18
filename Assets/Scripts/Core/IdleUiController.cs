using System;
using Animation;
using Interfaces;
using UnityEngine;
using UnityEngine.UI;

namespace Core
{
    public class IdleUiController : MonoBehaviour, IOnRestart, ILevelRegistryUser, ILevelState
    {
        private LevelRegistrySo _levelRegistrySo;
        [SerializeField] private Button backToMenuButton;

        private void Awake()
        {
            _levelRegistrySo.Register(this);
        }

        private void OnDestroy()
        {
            _levelRegistrySo.Unregister(this);
        }

        public void OnLevelRestart()
        {
            backToMenuButton.interactable = true;
        }

        public void LevelRegistrySoSetter(LevelRegistrySo levelRegistrySo)
        {
            _levelRegistrySo = levelRegistrySo;
        }

        public void OnLevelStart()
        {
            backToMenuButton.interactable = false;
        }

        public void OnLevelStop()
        {
            /*It will be empty*/
        }
    }
}