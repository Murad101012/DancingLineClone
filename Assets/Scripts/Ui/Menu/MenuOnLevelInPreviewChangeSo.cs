using System;
using Gameplay;
using UnityEngine;

namespace Ui.Menu
{
    [CreateAssetMenu(menuName = "ScriptableObjects/MenuLevelPreviewChangeEvent")]
    public class MenuOnLevelInPreviewChangeSo : ScriptableObject
    {
        public LevelPropertiesSo levelInPreview;
        public int levelIndexInPreview;
        public bool playerCurrentlyChangeLevelPreview;
        
        public event Action<bool> BeginLevelPreviewChangeEvent;
        public event Action LevelPreviewChangeEvent;

        private void OnEnable()
        {
            levelInPreview = null;
            levelIndexInPreview = -1;
        }

        public void OnBeginLevelPreviewChange()
        {
            BeginLevelPreviewChangeEvent?.Invoke(playerCurrentlyChangeLevelPreview);
        }

        public void ChangeLevelInPreview(LevelPropertiesSo levelPropertiesSo, int index, bool pass = false)
        {
            if (index == levelIndexInPreview && !pass) return;
            levelIndexInPreview = index;
            levelInPreview = levelPropertiesSo;
            LevelPreviewChangeEvent?.Invoke();
        }
    }
}