using System;
using System.Reflection;

namespace Refraction
{
    public static class TypeExtensions
    {
        public static TValue InvokeMember<TValue>(this Type type, string name, BindingFlags bindingFlags, params object[] arguments)
        {
            return (TValue)type.InvokeMember(name, bindingFlags, null, null, arguments);
        }

        public static void InvokeMember(this Type type, string name, BindingFlags bindingFlags, params object[] arguments)
        {
            type.InvokeMember(name, bindingFlags, null, null, arguments);
        }
    }
}
