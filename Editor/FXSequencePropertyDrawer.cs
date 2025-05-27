using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace PSkrzypa.UnityFX
{
    [CustomPropertyDrawer(typeof(FXSequence))]
    public class FXSequencePropertyDrawer : PropertyDrawer
    {
        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            // Use Unity’s default layout
            VisualElement root = base.CreatePropertyGUI(property);

            // Find the playMode field inside the default layout
            var playModeProp = property.FindPropertyRelative("playMode");

            // This is the field inside the default tree — safe to query by name
            var playModeField = root.Q<PropertyField>("playMode");

            if (playModeField != null)
            {
                playModeField.RegisterValueChangeCallback(_ =>
                {
                    var sequence = SerializedPropertyUtility.GetParentOfType<FXSequence>(property);
                    if (sequence != null)
                    {
                        float previousDuration = sequence.SequenceTiming?.Duration ?? 0f;
                        sequence.SequenceTiming?.RecalculateDuration(sequence.Components);
                        float newDuration = sequence.SequenceTiming?.Duration ?? 0f;
#if UNITY_EDITOR
                        if (previousDuration != newDuration)
                        {
                            UnityEditor.EditorUtility.SetDirty(property.serializedObject.targetObject);
                        }
#endif
                    }
                });
            }

            return root;
        }
    }


}