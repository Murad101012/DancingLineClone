using UnityEditor;
using UnityEngine;

namespace Core
{
    public class PianoAnimationSequencerWindow : EditorWindow
    {
        // 🎼 Consolidated raw timestamp stream arrays directly from your 14 structural groups
        private static readonly float[] TimelineTimestamps = new float[]
        {
            // Group 1 - 3
            31.6f, 32.0f, 32.5f, 33.0f, 33.5f, 34.2f, 34.46f, 34.6f, 34.9f, 35.1f, 35.39f, 36.0f, 36.3f, 36.6f, 36.8f, 37.0f, 37.3f,
            // Group 4 - 6
            39.21f, 39.73f, 40.21f, 40.7f, 41.1f, 41.88f, 42.1f, 42.3f, 42.5f, 42.8f, 43.08f, 43.8f, 44.05f, 44.7f, 45.0f,
            // Group 7 - 9
            45.96f, 46.52f, 46.95f, 47.43f, 47.9f, 48.41f, 48.7f, 48.86f, 49.55f, 49.89f, 50.1f, 50.35f, 50.66f, 50.78f,
            // Group 10 - 12
            51.52f, 51.74f, 52.45f, 52.7f, 53.6f, 54.1f, 54.6f, 55.1f, 55.55f, 56.0f, 56.53f,
            // Group 13 - 14
            57.2f, 57.58f, 57.87f, 58.0f, 58.2f, 58.4f, 59.6f, 59.96f, 60.19f, 60.43f
        };

        [MenuItem("Tools/Level Design/Piano Animation Sequencer")]
        public static void ShowWindow()
        {
            PianoAnimationSequencerWindow window = GetWindow<PianoAnimationSequencerWindow>("Animation Sequencer");
            window.minSize = new Vector2(450, 150);
        }

        private void OnGUI()
        {
            GUILayout.Label("Advanced Piano Animation Sequence Injector", EditorStyles.boldLabel);
            EditorGUILayout.Space(10);

            EditorGUILayout.HelpBox("Select the component container GameObject. This will APPEND fresh, paired (Hammer/String Shake) datasets to the end of the list and automatically apply timeline offsets.", MessageType.Info);

            EditorGUILayout.Space(10);

            LevelObjectAnimationController currentController = Selection.activeGameObject != null ? Selection.activeGameObject.GetComponent<LevelObjectAnimationController>() : null;
            GUI.enabled = currentController != null;

            GUI.backgroundColor = new Color(0.3f, 0.8f, 0.4f); // Clean Green Action Button
            if (GUILayout.Button("Generate and Inject Dynamic Sequences", GUILayout.Height(38)))
            {
                ExecuteSequenceInjection(currentController);
            }

            GUI.backgroundColor = Color.white;
            GUI.enabled = true;
        }

        private void ExecuteSequenceInjection(LevelObjectAnimationController controller)
        {
            if (controller == null) return;

            GameObject hammersParent = GameObject.Find("Piano_hammers");
            GameObject stringsParent = GameObject.Find("Piano_strings");

            if (hammersParent == null || stringsParent == null)
            {
                Debug.LogError("<color=red><b>Injection Failed:</b></color> Could not discover [Piano_hammers] or [Piano_strings] hierarchies.");
                return;
            }

            int hammerCount = hammersParent.transform.childCount;
            int stringCount = stringsParent.transform.childCount;
            int maxPairCount = Mathf.Min(hammerCount, stringCount);

            if (maxPairCount == 0)
            {
                Debug.LogWarning("Aborted Operation: Hierarchy children layout containers are empty.");
                return;
            }

            // 👑 NOTIFICATION LOGIC ENGINE: Analyze if data or objects are out of alignment bounds
            int timestampCount = TimelineTimestamps.Length;
            if (maxPairCount > timestampCount)
            {
                int missingTimestamps = maxPairCount - timestampCount;
                Debug.LogWarning($"<color=yellow><b>Notification:</b></color> Missing <b>{missingTimestamps}</b> timestamps! There are {maxPairCount} object pairs but only {timestampCount} timing entries. The final {missingTimestamps} pairs will not receive timeline values.");
            }
            else if (timestampCount > maxPairCount)
            {
                int leftoverTimestampsCount = timestampCount - maxPairCount;
                string leftoverList = "";
                for (int t = maxPairCount; t < timestampCount; t++)
                {
                    leftoverList += TimelineTimestamps[t] + (t < timestampCount - 1 ? ", " : "");
                }
                Debug.LogWarning($"<color=orange><b>Notification:</b></color> Too many timestamps given! Provided {timestampCount} values but only {maxPairCount} object pairs exist. Leftover numbers not processed: [{leftoverList}]");
            }

            SerializedObject serializedController = new SerializedObject(controller);
            serializedController.Update();

            SerializedProperty animationDataArrayProp = serializedController.FindProperty("animationData");
            if (animationDataArrayProp == null)
            {
                Debug.LogError("Failed serialization mapping: Field name 'animationData' could not be found.");
                return;
            }

            Undo.RegisterCompleteObjectUndo(controller, "Inject Complex Piano Sequences");

            int originalArraySize = animationDataArrayProp.arraySize;
            int spacesRequired = maxPairCount * 2;
            animationDataArrayProp.arraySize = originalArraySize + spacesRequired;

            Debug.Log($"Starting appending data blocks. First piano_hammer element begins exactly at **Element {originalArraySize}**.");

            int currentWritingIndex = originalArraySize;

            for (int i = 0; i < maxPairCount; i++)
            {
                Transform hammerChild = hammersParent.transform.GetChild(i);
                Transform stringChild = stringsParent.transform.GetChild(i);

                // Check if a timestamp is available for this sequence pass lane
                bool trackingTimestampAvailable = (i < timestampCount);
                float baseHammerTime = 0f;
                float baseStringTime = 0f;

                if (trackingTimestampAvailable)
                {
                    // Hammer gets raw value minus 0.8s pre-delay
                    baseHammerTime = TimelineTimestamps[i] - 0.8f;
                    // String plays right when the hammer hit completes: (BaseHammerTime + 0.8s duration)
                    baseStringTime = baseHammerTime + 0.8f; 
                }

                // ==========================================
                // 1. DYNAMIC PIANO HAMMER ENTRY CONFIGURATION
                // ==========================================
                SerializedProperty hammerElement = animationDataArrayProp.GetArrayElementAtIndex(currentWritingIndex);
                
                hammerElement.FindPropertyRelative("gameObjectTransform").objectReferenceValue = hammerChild;
                hammerElement.FindPropertyRelative("type").enumValueIndex = (int)LevelObjectAnimationController.AnimationType.Rotate;
                
                // 👑 NEW REQUIREMENT: Pull current active local rotation angles directly from hierarchy
                Vector3 currentHammerAngles = hammerChild.localEulerAngles;
                hammerElement.FindPropertyRelative("targetTransform").vector3Value = new Vector3(-90f, currentHammerAngles.y, currentHammerAngles.z);
                
                hammerElement.FindPropertyRelative("lifetime").enumValueIndex = (int)LevelObjectAnimationController.AnimationLifetime.Custom;
                hammerElement.FindPropertyRelative("duration").floatValue = 0.8f;
                hammerElement.FindPropertyRelative("repeatCount").intValue = 2;
                hammerElement.FindPropertyRelative("triggerType").enumValueIndex = (int)LevelObjectAnimationController.TriggerType.SoundtrackTimeline;
                hammerElement.FindPropertyRelative("triggerTypeValue").floatValue = baseHammerTime;

                currentWritingIndex++;

                // ==========================================
                // 2. DYNAMIC PIANO STRING ENTRY CONFIGURATION (SHAKE)
                // ==========================================
                SerializedProperty stringElement = animationDataArrayProp.GetArrayElementAtIndex(currentWritingIndex);
                
                stringElement.FindPropertyRelative("gameObjectTransform").objectReferenceValue = stringChild;
                
                // Assumes 'Shake' is the 4th item (Index 3) inside your updated AnimationType enum
                stringElement.FindPropertyRelative("type").enumValueIndex = 3; 
                
                stringElement.FindPropertyRelative("lifetime").enumValueIndex = (int)LevelObjectAnimationController.AnimationLifetime.OneTime;
                stringElement.FindPropertyRelative("duration").floatValue = 1.0f; // Force duration factor to 1
                stringElement.FindPropertyRelative("triggerType").enumValueIndex = (int)LevelObjectAnimationController.TriggerType.SoundtrackTimeline;
                stringElement.FindPropertyRelative("triggerTypeValue").floatValue = baseStringTime;

                currentWritingIndex++;
            }

            serializedController.ApplyModifiedProperties();
            
            Debug.Log($"<color=green><b>Success!</b></color> Appended {spacesRequired} interlaced animation tracks to the controller element list.");
        }
    }
}