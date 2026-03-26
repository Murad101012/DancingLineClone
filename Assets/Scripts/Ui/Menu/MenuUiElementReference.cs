using System;
using UnityEngine;
using UnityEngine.UIElements;

namespace Ui.Menu
{
    /// <summary>
    /// Provides references to UIToolkit VisualElements.
    /// </summary>
    /// <remarks>
    /// Centralizes string-based UI queries to prevent typos. 
    /// Use <see cref="CheckResult"/> to verify that all queried elements were successfully found in the UIDocument.
    /// </remarks>
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
        
        //TODO check this with Unit Testing if all buttons come correctly from UIToolkit
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

        /// <summary>
        /// Checks if a VisualElement is null and updates the global <see cref="CheckResult"/>.
        /// </summary>
        /// <param name="visualElement">The element to check.</param>
        /// <param name="nameOfVisualElement">The name to display in the warning log.</param>
        /// <remarks>
        /// <para>If the element is null, a <c>Debug.LogWarning</c> is issued and <see cref="CheckResult"/> is set to false.</para>
        /// <example>
        /// <code>
        /// Validate(myButton, nameof(myButton));
        /// </code>
        /// </example>
        /// </remarks>

        private void Validate(VisualElement visualElement, string nameOfVisualElement)
        {
            if (visualElement != null) return;
            Debug.LogWarning($"{name}: {nameOfVisualElement} is null");
            CheckResult = false;
        }
    }
}