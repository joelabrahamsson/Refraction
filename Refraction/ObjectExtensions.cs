using System.Reflection;

namespace Refraction
{
    public static class ObjectExtensions
    {
        public static TValue InvokeMember<TValue>(this object instance, string name, BindingFlags bindingFlags, params object[] arguments)
        {
            var type = instance.GetType();
            return (TValue)type.InvokeMember(name, bindingFlags, null, instance, arguments);
        }

        public static void InvokeMember(this object instance, string name, BindingFlags bindingFlags, params object[] arguments)
        {
            var type = instance.GetType();
            type.InvokeMember(name, bindingFlags, null, instance, arguments);
        }

        public static bool IsNull(this object value)
        {
            return value == null;
        }

        public static bool IsNotNull(this object value)
        {
            return !value.IsNull();
        }
    }
}