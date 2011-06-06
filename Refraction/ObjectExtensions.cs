using System.Reflection;

namespace Refraction
{
    public static class ObjectExtensions
    {
        public static TValue InvokeMember<TValue>(this object instance, string name, BindingFlags bindingFlags)
        {
            var type = instance.GetType();
            return (TValue)type.InvokeMember(name, bindingFlags, null, instance, null);
        }
    }
}