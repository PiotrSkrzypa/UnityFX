using System;

namespace PSkrzypa.UnityFX
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public sealed class FXComponentAttribute : Attribute
    {
        public FXComponentAttribute(string menuName)
        {
            MenuName = menuName;
        }

        public string MenuName { get; }
    }

}