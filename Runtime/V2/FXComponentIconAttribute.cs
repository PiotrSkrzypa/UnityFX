using System;

namespace PSkrzypa.UnityFX.V2
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public sealed class FXComponentIconAttribute : Attribute
    {
        public FXComponentIconAttribute(string iconPath)
        {
            IconPath = iconPath;
        }

        public string IconPath { get; }
    }

}