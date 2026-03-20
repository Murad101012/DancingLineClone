using System;
using UnityEngine;
using UnityEngine.UIElements;

namespace Ui.Menu
{
    /// <summary>
    /// It serves as having centralized UI element's naming. It prevents potential typos when getting reference from <see cref="MenuUiController"/>.
    /// If chancing name of an element "string value" required, it must be done from here.
    /// </summary>
    [RequireComponent(typeof(UIDocument))]
    public class MenuUiElementReference: MonoBehaviour
    {
        private VisualElement _root;
        
        private readonly string _levelLoadButtonName = "levelLoadButton";
        public Button LevelLoadButtonReference;
        
        /// <summary>
        /// Scripts those are using <see cref="MenuUiElementReference"/> must implement this event, to prevent null UI Element problems.
        /// True: All Visual Elements found in UIDocument/rootVisualElement
        /// False: One or more visual Elements couldn't find (Please check if corresponding element have string typo)
        /// </summary>
        public bool CheckResult { get; private set; }

        public bool CheckFinished { get; private set; }

        private void Awake()
        {
            //After we get rootVisualElement, we begin to processing if all elements mentioned in here are available (not null)
            _root = GetComponent<UIDocument>().rootVisualElement;
            
            Initialization();
        }
        
        private void Initialization()
        {
            LevelLoadButtonReference = _root.Q<Button>(_levelLoadButtonName);
            
            //We're beginning with true, otherwise each time null check make checkResult to true even one of the Null check find problem,
            //it might be overridden  it 
            CheckResult = true;
            
            Validate(LevelLoadButtonReference, nameof(LevelLoadButtonReference));
            
            
            if (!CheckResult)
            {
                Debug.LogError($"{name}: One or more visual Elements couldn't find" +
                               " (Please check if corresponding element have string typo)");
            }

            CheckFinished = true;
        }

        /// <param name="nameOfVisualElement">Checking nameOf inside the Validate() cause wrong string name,
        /// so it must be added as parameter</param>
        private void Validate(VisualElement visualElement, string nameOfVisualElement)
        {
            if (visualElement != null) return;
            Debug.LogWarning($"{name}: {nameOfVisualElement} is null");
            CheckResult = false;
        }
    }
}