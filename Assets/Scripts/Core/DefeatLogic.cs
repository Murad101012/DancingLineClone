using Interfaces;
using UnityEngine;

namespace Core
{
    public class DefeatLogic : MonoBehaviour, ILevelState, IOnRestart, IOnCheckPoint
    {
        public void OnLevelStart()
        {
            throw new System.NotImplementedException();
        }

        public void OnLevelStop()
        {
            throw new System.NotImplementedException();
        }

        public void OnLevelRestart()
        {
            throw new System.NotImplementedException();
        }

        public void OnLevelCheckPoint()
        {
            throw new System.NotImplementedException();
        }
    }
}