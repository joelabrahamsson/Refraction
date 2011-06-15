using System;
using System.Linq;
using System.Reflection;

namespace Refraction
{
    public static class AssemblyExtensions
    {
        public static Type GetTypeNamed(this Assembly assembly, string name)
        {
            var foundClass = assembly.GetTypes().Where(t => t.Name.Equals(name)).FirstOrDefault();
            if (foundClass == default(Type))
            {
                throw new Exception(string.Format("Assembly does not contain a class named {0}", name));
            }
            return assembly.GetTypes().Where(t => t.Name.Equals(name)).First();
        }

        public static object GetTypeInstance(this Assembly assembly, string name, params object[] ctorArguments)
        {
            var type = assembly.GetTypeNamed(name);
            return Activator.CreateInstance(type, ctorArguments);
        }
    }
}