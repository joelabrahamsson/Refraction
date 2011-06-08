using System;
using System.Collections.Generic;
using System.Linq;
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

        public static IEnumerable<Type> GetBaseTypes(this Type type)
        {
            if(type.IsNull() || type == typeof(object))
            {
                return new List<Type>();
            }
            var baseTypes = type.GetInterfaces().Where(x => x.IsNotNull()).ToList();
            if (type.BaseType.IsNotNull())
            {
                baseTypes.Add(type.BaseType);
            }

            var grandparents = new List<Type>(baseTypes);
            foreach (var baseType in baseTypes)
            {
                grandparents.AddRange(baseType.GetBaseTypes());
            }

            return baseTypes;
        }
    }
}
