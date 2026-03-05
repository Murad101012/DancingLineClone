using System;
using Gameplay;
using Interfaces;
using Player;
using UnityEngine;

namespace Core
{
    public class LevelStateManager : MonoBehaviour, ILevelEntity
    {
        [SerializeField] private LevelRegistrySo register;
        [SerializeField] private LevelPropertiesSo levelPropertiesSo;
        public static Action<Transform> CheckPointEvent;
        [SerializeField] private GameObject levelBeginButton; //:TODO Find a better location for this button

        private void OnEnable()
        {
            CheckPointEvent += CheckPointSetter;
            Movement.Dead += OnStopTheGame;
            register.Register(this);
        }

        private void Awake()
        {
            
        }

        private void OnDisable()
        {
            CheckPointEvent -= CheckPointSetter;
            Movement.Dead -= OnStopTheGame;
            register.Unregister(this);
        }

        private void CheckPointSetter(Transform checkPoint)
        {
            Debug.Log(checkPoint.position + "/" + checkPoint.rotation.eulerAngles);
        }

        public void StartTheGame()
        {
            register.TriggerStart();
        }

        public void RestartTheGame()
        {
            register.TriggerRestart();
        }

        private void OnStopTheGame()
        {
            register.TriggerStop();
        }

        public void OnLevelStart()
        {
            levelBeginButton.SetActive(false);
        }

        public void OnLevelStop()
        {
            //It will be empty
        }

        public void OnLevelRestart()
        {
            levelBeginButton.SetActive(true);
        }
    }
}