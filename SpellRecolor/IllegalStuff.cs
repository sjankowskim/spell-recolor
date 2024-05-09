using System.Reflection;

namespace SpellRecolor
{
    static class IllegalStuff
    {
        public static T GetField<T>(this object instance, string fieldName)
        {
            if (instance == null) return default;
            BindingFlags bindFlags = BindingFlags.Instance
                                     | BindingFlags.Public
                                     | BindingFlags.NonPublic
                                     | BindingFlags.Static;
            FieldInfo field = instance.GetType().GetField(fieldName, bindFlags);
            return (T)field.GetValue(instance);
        }
        public static void SetField<T, U>(this T instance, string fieldName, U value)
        {
            BindingFlags bindFlags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic
                | BindingFlags.Static;
            FieldInfo field = instance.GetType().GetField(fieldName, bindFlags);
            field.SetValue(instance, value);
        }
    }
}
