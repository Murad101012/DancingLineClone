using System;
using Player;
using UnityEngine;

namespace Core
{
    /// <summary>
    /// Operator script that manages the game
    /// </summary>
    public class GameManager : MonoBehaviour
    {
        public bool dead;

        private void OnEnable()
        {
            Movement.Dead += Death;
        }

        private void Awake()
        {
            DontDestroyOnLoad(gameObject);
        }

        private void OnDisable()
        {
            Movement.Dead -= Death;
        }

        private void Death()
        {
            dead = true;
        }
    }
}