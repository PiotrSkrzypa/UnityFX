using System;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;

namespace PSkrzypa.UnityFX.Editor
{
    public sealed class AddFXComponentDropdown : AdvancedDropdown
    {
        class Item : AdvancedDropdownItem
        {
            public Item(Type type, string menuName) : base(menuName)
            {
                Type = type;
            }

            public Type Type { get; }
        }

        static readonly (Type Type, string MenuName)[] cache = TypeCache.GetTypesDerivedFrom<FXComponent>()
            .Where(x => !x.IsAbstract)
            .Where(x => !x.IsSpecialName)
            .Where(x => !x.IsGenericType)
            .Select(x =>
            {
                var attribute = x.GetCustomAttribute<FXComponentAttribute>();
                var menuName = attribute == null ? x.Name : attribute.MenuName;
                return (x, menuName);
            })
            .OrderBy(x => x.menuName)
            .ToArray();

        public event Action<Type> OnTypeSelected;

        public AddFXComponentDropdown(AdvancedDropdownState state) : base(state)
        {
            var minimumSize = this.minimumSize;
            minimumSize.y = 200f;
            this.minimumSize = minimumSize;
        }

        protected override AdvancedDropdownItem BuildRoot()
        {
            var root = new AdvancedDropdownItem("Component");
            foreach ((var type, var menuName) in cache)
            {
                var splitStrings = menuName.Split('/');
                var parent = root;
                Item lastItem = null;

                for (int i = 0; i < splitStrings.Length; i++)
                {
                    var str = splitStrings[i];

                    var foundChildItem = parent.children.FirstOrDefault(item => item.name == str);
                    if (foundChildItem != null)
                    {
                        parent = foundChildItem;
                        lastItem = (Item)foundChildItem;
                        continue;
                    }

                    var child = new Item(type, str);

                    if (i == splitStrings.Length - 1)
                    {
                        var attribute = type.GetCustomAttribute<FXComponentIconAttribute>();
                        var iconPath = attribute == null ? "d_Animation Icon" : attribute.IconPath;
                        child.icon = (Texture2D)EditorGUIUtility.IconContent(iconPath).image;
                    }

                    parent.AddChild(child);

                    parent = child;
                    lastItem = child;
                }
            }

            return root;
        }

        protected override void ItemSelected(AdvancedDropdownItem item)
        {
            base.ItemSelected(item);

            if (item is Item componentItem)
            {
                OnTypeSelected?.Invoke(componentItem.Type);
            }
        }
    }
}