using System.Collections;
using System.Linq;
using System.Reflection;
using UnityEditor;

namespace PSkrzypa.UnityFX
{
    public static class SerializedPropertyUtility
    {
        public static T GetParentOfType<T>(SerializedProperty property) where T : class
        {
            object current = property.serializedObject.targetObject;
            string[] pathParts = property.propertyPath.Replace(".Array.data[", "[").Split('.');

            foreach (string part in pathParts.Take(pathParts.Length - 1))
            {
                if (part.Contains("["))
                {
                    string fieldName = part.Substring(0, part.IndexOf("["));
                    int index = int.Parse(part.Substring(part.IndexOf("[")).Replace("[", "").Replace("]", ""));

                    var list = GetFieldOrPropertyValue(current, fieldName) as IList;
                    if (list == null || index >= list.Count) return null;

                    current = list[index];
                }
                else
                {
                    current = GetFieldOrPropertyValue(current, part);
                }

                if (current is T typed)
                    return typed;
            }

            return null;
        }

        private static object GetFieldOrPropertyValue(object source, string name)
        {
            if (source == null) return null;

            var type = source.GetType();
            var field = type.GetField(name, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            if (field != null)
                return field.GetValue(source);

            var prop = type.GetProperty(name, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            if (prop != null)
                return prop.GetValue(source);

            return null;
        }
    }

}
