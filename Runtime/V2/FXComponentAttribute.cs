using System;

namespace PSkrzypa.UnityFX.V2
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