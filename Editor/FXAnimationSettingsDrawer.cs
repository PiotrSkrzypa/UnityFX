using System;
using System.Collections.Generic;
using Unity.Collections;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace PSkrzypa.UnityFX.V2.Editor
{
    [CustomPropertyDrawer(typeof(FXAnimationSettings))]
    public class FXAnimationSettingsDrawer : PropertyDrawer
    {
        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            var foldout = new Foldout()
            {
                text = property.displayName,
                style = {
                    marginLeft = 15f,
                    paddingRight = 3f,
                    alignSelf = Align.Stretch,
                }
            };

            foldout.BindProperty(property);

            Group(foldout, group =>
            {
                AddPropertyField(group, property, "duration");
            });

            Group(foldout, group =>
            {
                AddPropertyField(group, property, "delay");
                AddPropertyField(group, property, "delayType");
            });

            Group(foldout, group =>
            {
                AddPropertyField(group, property, "loops");
                AddPropertyField(group, property, "loopType");
            });

            Group(foldout, group =>
            {
                AddPropertyField(group, property, "ease");
            });

            Group(foldout, group =>
            {
                AddPropertyField(group, property, "timescaleIndependent");
            });

            Group(foldout, group =>
            {
                AddPropertyField(group, property, "forwardPlaybackSpeed");
                AddPropertyField(group, property, "backwardPlaybackSpeed");
            });


            return foldout;
        }

        void Group(VisualElement root, Action<VisualElement> configure)
        {
            var group = new UnityEngine.UIElements.Box() { style = { marginBottom = 3f } };
            configure(group);
            root.Add(group);
        }

        void AddPropertyField(VisualElement root, SerializedProperty property, string name)
        {
            root.Add(new PropertyField(property.FindPropertyRelative(name)));
        }
    }
}
