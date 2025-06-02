using System.Collections;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace PSkrzypa.UnityFX.Editor
{
    [CustomPropertyDrawer(typeof(FXTiming), true)]
    public class FXTimingPropertyDrawer : PropertyDrawer
    {
        public VisualTreeAsset visualTree;
        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            VisualElement root = new VisualElement();
            visualTree.CloneTree(root);
            FloatField durationField = BindField<FloatField>(root, property, "Duration");
            FloatField initialDelayField = BindField<FloatField>(root, property, "InitialDelay");
            FloatField cooldownDurationField = BindField<FloatField>(root, property, "CooldownDuration");
            FloatField delayBetweenRepeatsField = BindField<FloatField>(root, property, "DelayBetweenRepeats");
            IntegerField numberOfRepeatsField = BindField<IntegerField>(root, property, "NumberOfRepeats");
            Toggle contributeToTotalDurationToggle = BindField<Toggle>(root, property, "ContributeToTotalDuration");
            Toggle repeatForeverField = BindField<Toggle>(root, property, "RepeatForever");
            BindField<Toggle>(root, property, "TimeScaleIndependent");
            if (property.type == "FXSequenceTiming")
            {
                durationField.SetEnabled(false);
                var field = new PropertyField(property.FindPropertyRelative("playMode"));
                field.name = "playMode";
                field.RegisterValueChangeCallback(_ => OnTimingModified(property));
                root.Insert(0, field);
            }
            ClampFloat(root, "Duration");
            ClampFloat(root, "InitialDelay");
            ClampFloat(root, "CooldownDuration");
            ClampFloat(root, "DelayBetweenRepeats");
            ClampInt(root, "NumberOfRepeats");


            if (repeatForeverField != null && numberOfRepeatsField != null)
            {
                // Initial state
                numberOfRepeatsField.SetEnabled(!repeatForeverField.value);

                // Respond to toggle change
                repeatForeverField.RegisterValueChangedCallback(evt =>
                {
                    numberOfRepeatsField.SetEnabled(!evt.newValue);
                });
            }
            durationField.RegisterValueChangedCallback(evt =>
            {
                OnTimingModified(property);
            });
            initialDelayField.RegisterValueChangedCallback(evt =>
            {
                OnTimingModified(property);
            });
            cooldownDurationField.RegisterValueChangedCallback(evt =>
            {
                OnTimingModified(property);
            });
            delayBetweenRepeatsField.RegisterValueChangedCallback(evt =>
            {
                OnTimingModified(property);
            });
            numberOfRepeatsField.RegisterValueChangedCallback(evt =>
            {
                OnTimingModified(property);
            });
            contributeToTotalDurationToggle.RegisterValueChangedCallback(evt =>
            {
                OnTimingModified(property);
            });
            return root;
        }

        private static void OnTimingModified(SerializedProperty property)
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
        }

        private T BindField<T>(VisualElement root, SerializedProperty parent, string fieldName)
    where T : BindableElement
        {
            var field = root.Q<T>(fieldName);
            var relative = parent.FindPropertyRelative(fieldName);
            field?.BindProperty(relative);
            return field;
        }
        private static void ClampFloat(VisualElement root, string elementName)
        {
            FloatField duration = root.Q<FloatField>(elementName);
            duration.RegisterValueChangedCallback(evt =>
            {
                float clamped = evt.newValue < 0 ? 0 : evt.newValue;
                if (!Mathf.Approximately(evt.newValue, clamped))
                    duration.value = clamped;
            });
        }
        private static void ClampInt(VisualElement root, string elementName)
        {
            IntegerField duration = root.Q<IntegerField>(elementName);
            duration.RegisterValueChangedCallback(evt =>
            {
                int clamped = evt.newValue < 0 ? 0 : evt.newValue;
                if (!Mathf.Approximately(evt.newValue, clamped))
                    duration.value = clamped;
            });
        }

        private object GetValue(object source, string name)
        {
            if (source == null) return null;
            var type = source.GetType();
            var field = type.GetField(name, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
            return field?.GetValue(source);
        }
    }

}