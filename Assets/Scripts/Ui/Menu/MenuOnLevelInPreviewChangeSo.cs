using System;
using Gameplay;
using UnityEngine;

namespace Ui.Menu
{
    [CreateAssetMenu(menuName = "ScriptableObjects/MenuLevelPreviewChangeEvent")]
    public class MenuOnLevelInPreviewChangeSo : ScriptableObject
    {
        public LevelPropertiesSo levelInPreview;
        public event Action LevelPreviewChangeEvent;

        private void OnEnable()
        {
            levelInPreview = null;
        }

        public void ChangeLevelInPreview(LevelPropertiesSo levelPropertiesSo)
        {
            if (levelPropertiesSo == levelInPreview) return;
            levelInPreview = levelPropertiesSo;
            LevelPreviewChangeEvent?.Invoke();
        }
    }
}