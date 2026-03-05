using System;
using Interfaces;
using Player;
using UnityEngine;

namespace Core
{
    public class DefeatLogic : MonoBehaviour, ILevelEntity
    {
        [SerializeField] private GameObject defeatScreen;
        [SerializeField] private LevelRegistrySo registry;
        
        private void OnEnable()
        {
            Movement.Dead += Defeated;
            registry.Register(this);
        }

        private void OnDisable()
        {
            Movement.Dead -= Defeated;
            registry.Unregister(this);
        }

        private void Defeated()
        {
            defeatScreen.SetActive(true);
        }

        public void OnLevelStart()
        {
            //It will be empty
        }

        public void OnLevelStop()
        {
            //It will be empty
        }

        public void OnLevelRestart()
        {
            defeatScreen.SetActive(false);
        }
    }
}