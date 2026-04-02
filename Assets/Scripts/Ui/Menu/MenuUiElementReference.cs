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

        private readonly string _filmLevelName = "Cont_FilmLevel";
        public VisualElement FilmLevelReference;
        
        private readonly string _levelLabelName = "Lbl_LevelTitle";
        public Label LevelLabelNameReference;
        
        //Deprecated
        private readonly string _levelChangeNextLevelButtonName = "Btn_NextLevel";
        public Button LevelChangeNextLevelButtonReference;
        
        private readonly string _levelChangePreviousLevelButtonName = "Btn_PrevLevel";
        public Button LevelChangePreviousLevelButtonReference;
        
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
        
        //TODO check this with Unit Testing if all buttons come correctly from UIToolkit
        private void Initialization()
        {
            //List all elements to get reference
            //LevelLoadButtonReference = _root.Q<Button>(_levelLoadButtonName);
            LevelLabelNameReference = Root.Q<Label>(_levelLabelName);
            LevelChangeNextLevelButtonReference = Root.Q<Button>(_levelChangeNextLevelButtonName);
            LevelChangePreviousLevelButtonReference = Root.Q<Button>(_levelChangePreviousLevelButtonName);
            FilmLevelReference = Root.Q<VisualElement>(_filmLevelName);
            
            
            //We're beginning with true, otherwise each time null check make checkResult to true even one of the Null check find problem,
            //it might be overridden  it 
            CheckResult = true;
            
            //Validate(LevelLoadButtonReference, nameof(LevelLoadButtonReference));
            Validate(LevelLabelNameReference, nameof(LevelLabelNameReference));
            Validate(LevelChangeNextLevelButtonReference, nameof(LevelChangeNextLevelButtonReference));
            Validate(LevelChangePreviousLevelButtonReference, nameof(LevelChangePreviousLevelButtonReference));
            Validate(FilmLevelReference, nameof(FilmLevelReference));
            
            
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