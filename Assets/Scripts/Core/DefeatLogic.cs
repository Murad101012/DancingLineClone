using System;
using Interfaces;
using UnityEngine;

namespace Core
{
    /// <summary>
    /// Logic when player dead
    /// </summary>
    public class DefeatLogic : MonoBehaviour, IOnDead, ILevelRegistryUser
    {
       private DefeatUiController _defeatUiController;
       [SerializeField] private ProgressInCurrentLoadedLevel progressInCurrentLoadedLevel;
       
       private LevelRegistrySo _levelRegistrySo;

       private void Awake()
       {
           _levelRegistrySo.Register(this);
           
           TryGetComponent(out _defeatUiController);
       }

       private void OnDestroy()
       {
           _levelRegistrySo.Unregister(this);
       }

       public void OnDead()
       {
           if (_defeatUiController != null)
           {
               //On dead, it calculates the progress (as %) player made by "playback time of current song * 100 / total length of current song"
               _defeatUiController.ChangeLevelProgressText(Mathf.Clamp(progressInCurrentLoadedLevel.playbackInAudioWhenPlayerDead * 100 / progressInCurrentLoadedLevel.audioDuration, 0f, 100f));
           }
           else
           {
               Debug.LogWarning($"{name}: {nameof(_defeatUiController)} is null. Can't change progress in defeat screen that how much player progress");
           }
       }

       public void LevelRegistrySoSetter(LevelRegistrySo levelRegistrySo)
       {
           _levelRegistrySo = levelRegistrySo;
       }
    }
}