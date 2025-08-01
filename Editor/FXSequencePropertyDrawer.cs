using UnityEngine.UIElements;
using UnityEditor;
using UnityEditor.UIElements;
using System.Collections.Generic;
using UnityEngine;
using LitMotion;
using System.Reflection;

namespace PSkrzypa.UnityFX.V2.Editor
{
    [CustomPropertyDrawer(typeof(FXSequence))]
    public sealed class FXSequencePropertyDrawer : PropertyDrawer
    {
        SerializedProperty componentsProperty;
        int prevArraySize;

        AddFXComponentDropdown dropdown;
        VisualElement componentRoot;
        SerializedObject target;
        bool updateBars;


        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            var root = new VisualElement();
            componentRoot = new VisualElement();
            target = property.serializedObject;

            componentsProperty = property.FindPropertyRelative("components");
            prevArraySize = componentsProperty.arraySize;

            dropdown = new AddFXComponentDropdown(new());
            dropdown.OnTypeSelected += type =>
            {
                var last = componentsProperty.arraySize;
                componentsProperty.InsertArrayElementAtIndex(componentsProperty.arraySize);
                var property = componentsProperty.GetArrayElementAtIndex(last);
                property.managedReferenceValue = ReflectionHelper.CreateDefaultInstance(type);
                property.serializedObject.ApplyModifiedProperties();
            };
            root.Add(new PropertyField(property.FindPropertyRelative("displayName")));
            root.Add(CreateSettingsPanel(property));
            componentRoot.Add(CreateComponentsPanel(property.serializedObject));
            root.Add(componentRoot);
            root.Add(CreateCallbacksPanel(property));
            if (property.depth == 0)
            {
                root.Add(CreateDebugPanel()); 
            }

            return root;
        }

        void OnEnable()
        {
            EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
        }

        void OnDisable()
        {
            if (!EditorApplication.isPlayingOrWillChangePlaymode && target != null)
            {
                ReflectionHelper.Invoke(target.targetObject, "Stop", null);
            }

            EditorApplication.playModeStateChanged -= OnPlayModeStateChanged;
        }

        void OnPlayModeStateChanged(PlayModeStateChange state)
        {
            if (state == PlayModeStateChange.ExitingEditMode)
            {
                ReflectionHelper.Invoke(target.targetObject, "Stop", null);
            }
        }

        VisualElement CreateBox(string label)
        {
            var box = new Box
            {
                style = {
                    marginTop = 6f,
                    marginBottom = 2f,
                    paddingLeft = 4f,
                    alignItems = Align.Stretch,
                    flexDirection = FlexDirection.Column,
                    flexGrow = 1f,
                }
            };

            box.Add(new Label(label)
            {
                style = {
                    marginTop = 5f,
                    marginBottom = 3f,
                    unityFontStyleAndWeight = FontStyle.Bold
                }
            });

            return box;
        }

        VisualElement CreateSettingsPanel(SerializedProperty serializedProperty)
        {
            SerializedObject serializedObject = serializedProperty.serializedObject;
            var box = CreateBox("Sequence Settings");
            box.Add(new PropertyField(serializedObject.FindProperty("playOnAwake")));
            box.Add(new PropertyField(serializedProperty.FindPropertyRelative("settings")));

            var toggle = new Toggle("Update Progress Bars");
            toggle.value = updateBars;
            toggle.RegisterValueChangedCallback(evt =>
            {
                updateBars = evt.newValue;
            });
            box.Add(toggle);
            return box;
        }

        VisualElement CreateComponentsPanel(SerializedObject serializedObject)
        {
            var box = CreateBox("Components");
            var views = new List<FXComponentView>();

            for (int i = 0; i < componentsProperty.arraySize; i++)
            {
                var property = componentsProperty.GetArrayElementAtIndex(i);
                var view = CreateComponentGUI(property.Copy());
                CreateContextMenuManipulator(componentsProperty, i, false).target = view.Foldout.Q<Toggle>();

                var enabledProperty = property.FindPropertyRelative("enabled");
                if (enabledProperty != null)
                {
                    view.EnabledToggle.BindProperty(enabledProperty);
                }

                box.Add(view);
                views.Add(view);
                CreateContextMenuManipulator(componentsProperty, i, true).target = view.ContextMenuButton;
            }

            var addButton = new Button()
            {
                text = "Add...",
                style = {
                    width = 200f,
                    alignSelf = Align.Center
                }
            };
            addButton.clicked += () => dropdown.Show(addButton.worldBound);
            box.Add(addButton);

            box.schedule.Execute(() =>
            {
                var enabled = IsActive();
                foreach (var view in views)
                {
                    view.SetEnabled(enabled);
                }
                addButton.SetEnabled(enabled);
            }).Every(30);

            box.schedule.Execute(() =>
            {
                if (componentsProperty.arraySize != prevArraySize)
                {
                    RefleshComponentsView(true, serializedObject);
                    prevArraySize = componentsProperty.arraySize;
                }
                if (!updateBars) return;
                for (int i = 0; i < views.Count; i++)
                {
                    SerializedProperty componentProperty = componentsProperty.GetArrayElementAtIndex(i);
                    if (prevArraySize <= i)
                    {
                        views[i].Progress = 0f;
                        continue;
                    }

                    if (componentProperty == null)
                    {
                        views[i].Progress = 0f;
                        continue;
                    }
                    FXComponent fXComponent = componentProperty.boxedValue as FXComponent;
                    if (fXComponent.IsPlaying)
                    {
                        views[i].Progress = fXComponent.Progress; 
                    }
                    else
                    {
                        views[i].Progress = 0f;
                    }
                }
            })
            .Every(30);

            return box;
        }
        VisualElement CreateCallbacksPanel(SerializedProperty serializedProperty)
        {
            SerializedObject serializedObject = serializedProperty.serializedObject;
            var box = CreateBox("Callbacks");
            var foldout = new Foldout()
            {
                text = "OnComplete",
                style = {
                    marginLeft = 15f,
                    paddingRight = 3f,
                    alignSelf = Align.Stretch,
                }
            };
            foldout.Add(new PropertyField(serializedProperty.FindPropertyRelative("onComplete")));
            box.Add(foldout);
            return box;
        }
        VisualElement CreateDebugPanel()
        {
            var box = CreateBox("Debug");
            var buttonGroup = new VisualElement
            {
                style = {
                    flexDirection = FlexDirection.Row,
                    flexGrow = 1f,
                }
            };
            var playButton = new Button(() => ReflectionHelper.Invoke(target.targetObject, "Play", null))
            {
                text = "Play",
                style = {
                    flexGrow = 1f,
                }
            };
            var rewindButton = new Button(() => ReflectionHelper.Invoke(target.targetObject, "Rewind", null))
            {
                text = "Rewind",
                style = {
                    flexGrow = 1f,
                }
            };
            var pasueButton = new Button(() => ReflectionHelper.Invoke(target.targetObject, "Pause", null))
            {
                text = "Pause",
                style = {
                    flexGrow = 1f,
                }
            };
            var stopButton = new Button(() => ReflectionHelper.Invoke(target.targetObject, "Stop", null))
            {
                text = "Stop",
                style = {
                    flexGrow = 1f,
                }
            };

            buttonGroup.Add(playButton);
            buttonGroup.Add(rewindButton);
            buttonGroup.Add(pasueButton);
            buttonGroup.Add(stopButton);

            buttonGroup.schedule.Execute(() =>
            {
                var enabled = !IsActive();
                pasueButton.SetEnabled(enabled);
                stopButton.SetEnabled(enabled);
            })
            .Every(30);

            box.Add(buttonGroup);

            return box;
        }

        void RefleshComponentsView(bool applyModifiedProperties, SerializedObject serializedObject)
        {
            if (applyModifiedProperties)
            {
                serializedObject.ApplyModifiedProperties();
            }

            componentRoot.Clear();
            componentRoot.Add(CreateComponentsPanel(serializedObject));
        }

        FXComponentView CreateComponentGUI(SerializedProperty property)
        {
            var view = new FXComponentView();

            if (string.IsNullOrEmpty(property.managedReferenceFullTypename))
            {
                view.Text = "(Missing)";
                view.Icon = (Texture2D)EditorGUIUtility.IconContent("Error").image;
                view.EnabledToggle.value = true;
                view.SetEnabled(true);
                view.EnabledToggle.Q("unity-checkmark").style.visibility = Visibility.Hidden;
                view.Add(new HelpBox("The type referenced in SerializeReference is missing. You may have renamed the type or moved it to a different namespace or assembly.", HelpBoxMessageType.Error));
            }
            else
            {
                view.Text = property.FindPropertyRelative("displayName").stringValue;

                var attribute = property.boxedValue.GetType().GetCustomAttribute<FXComponentIconAttribute>();
                var iconPath = attribute == null ? "d_Animation Icon" : attribute.IconPath;
                view.Icon = (Texture2D)EditorGUIUtility.IconContent(iconPath).image;


                view.TrackPropertyValue(property.FindPropertyRelative("displayName"), x =>
                {
                    view.Text = x.stringValue;
                });

                if (property.managedReferenceFullTypename.Contains("FXSequence"))
                {
                    view.Foldout.Add(new PropertyField(property));
                    return view;
                }
                view.Foldout.BindProperty(property);

                var endProperty = property.GetEndProperty();
                var isFirst = true;
                while (property.NextVisible(isFirst))
                {
                    if (SerializedProperty.EqualContents(property, endProperty)) break;
                    if (property.name == "enabled") continue;
                    isFirst = false;

                    view.Add(new PropertyField(property));
                }
            }

            return view;
        }

        ContextualMenuManipulator CreateContextMenuManipulator(SerializedProperty property, int arrayIndex, bool activeLeftClick)
        {
            var manipulator = new ContextualMenuManipulator(evt =>
            {
                evt.menu.AppendAction("Reset", x =>
                {
                    Undo.RecordObject(property.serializedObject.targetObject, "Reset LitMotionAnimation component");
                    var elementProperty = property.GetArrayElementAtIndex(arrayIndex);
                    elementProperty.managedReferenceValue = ReflectionHelper.CreateDefaultInstance(elementProperty.managedReferenceValue.GetType());
                    RefleshComponentsView(true, property.serializedObject);
                }, string.IsNullOrEmpty(property.GetArrayElementAtIndex(arrayIndex).managedReferenceFullTypename) ? DropdownMenuAction.Status.Disabled : DropdownMenuAction.Status.Normal);

                evt.menu.AppendSeparator();

                evt.menu.AppendAction("Remove Component", x =>
                {
                    property.DeleteArrayElementAtIndex(arrayIndex);
                    RefleshComponentsView(true, property.serializedObject);
                });

                evt.menu.AppendAction("Move Up", x =>
                {
                    property.MoveArrayElement(arrayIndex, arrayIndex - 1);
                    RefleshComponentsView(true, property.serializedObject);
                }, arrayIndex == 0 ? DropdownMenuAction.Status.Disabled : DropdownMenuAction.Status.Normal);


                evt.menu.AppendAction("Move Down", x =>
                {
                    property.MoveArrayElement(arrayIndex, arrayIndex + 1);
                    RefleshComponentsView(true, property.serializedObject);
                }, arrayIndex == property.arraySize - 1 ? DropdownMenuAction.Status.Disabled : DropdownMenuAction.Status.Normal);
            });

            if (activeLeftClick)
            {
                manipulator.activators.Add(new ManipulatorActivationFilter
                {
                    button = MouseButton.LeftMouse,
                });
            }

            return manipulator;
        }

        bool IsActive()
        {
            return !ReflectionHelper.GetValueBool(target.targetObject, "IsPlaying");
        }
    }
}