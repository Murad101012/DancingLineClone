using System;
using UnityEngine;

namespace Core
{
    public class VictoryLogic : MonoBehaviour
    {
        //TODO Made an event only for RestartButton trigger, found another way to connect them 
        public static event Action OnRestartButtonPressed;
        public void RestartButton()
        {
            OnRestartButtonPressed?.Invoke();
        }
    }
}