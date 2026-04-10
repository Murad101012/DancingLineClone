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
    /// To successfully find Visual Element from UXML, the name of UI Element must match variable name
    /// at <see cref="Initialization"/>. For each referencing it <c>MUST BE</c> adding <see cref="Validate"/>,
    /// otherwise type-checking will be skips for that element.
    /// </remarks>
    /// <example>If <c><![CDATA[<ui:Button name="levelLoadButton"/>]]></c> 
    /// then <see cref="_levelLoadButtonName"/> = "levelLoadButton";</example>
    [RequireComponent(typeof(UIDocument))]
    public class MenuUiElementReference: MonoBehaviour
    {
        public VisualElement Root;
        
        private readonly string _dragZoneName = "Cont_DragZone";
        public VisualElement DragZoneReference;

        private readonly string _carouselName = "Cont_Carousel";
        public VisualElement CarouselReference;
        
        private readonly string _levelLabelName = "Lbl_LevelTitle";
        public Label LevelLabelNameReference;

        private readonly string _debugLabelName = "Lbl_DebugText";
        public Label DebugLabelNameReference;
        
        public VisualElement[] LevelButtonsReferences;
        
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
            Root = GetComponent<UIDocument>().rootVisualElement;
            
            Initialization();
        }
        
        private void Initialization()
        {
            //List all elements to get reference
            //LevelLoadButtonReference = _root.Q<Button>(_levelLoadButtonName);
            LevelLabelNameReference = Root.Q<Label>(_levelLabelName);
            CarouselReference = Root.Q<VisualElement>(_carouselName);
            DebugLabelNameReference = Root.Q<Label>(_debugLabelName);
            DragZoneReference = Root.Q<VisualElement>(_dragZoneName);
            
            LevelButtonsReferences = new VisualElement[CarouselReference.childCount];
            for (int i = 0; i < CarouselReference.childCount; i++)
            {
                LevelButtonsReferences[i] = CarouselReference[i]; 
            }
            
            
            //We're beginning with true, otherwise each time null check make checkResult to true even one of the Null check find problem,
            //it might be overridden  it 
            CheckResult = true;
            
            //Validate(LevelLoadButtonReference, nameof(LevelLoadButtonReference));
            Validate(LevelLabelNameReference, nameof(LevelLabelNameReference));
            Validate(CarouselReference, nameof(CarouselReference));
            Validate(DebugLabelNameReference, nameof(DebugLabelNameReference));
            Validate(DragZoneReference, nameof(DragZoneReference));
            for (int i = 0; i < CarouselReference.childCount; i++)
            {
                Validate(LevelButtonsReferences[i], nameof(LevelButtonsReferences)); 
            }
            
            
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