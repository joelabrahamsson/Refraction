using System;
using System.CodeDom;
using System.Reflection;

namespace Refraction
{
    public static class Create
    {
        public const string DefaultNamespaceName = "DefaultNamespace";

        public static Assembly Assembly(Action<AssemblyDefinition> with)
        {
            var assembly = new AssemblyDefinition();
            with(assembly);
            return assembly.BuildAssembly();
        }

        public static CodeTypeDeclaration Class(this CodeCompileUnit assembly, string className)
        {
            var defaultNamespace = new CodeNamespace(DefaultNamespaceName);
            defaultNamespace.Imports.Add(new CodeNamespaceImport("System"));
            assembly.Namespaces.Add(defaultNamespace);
            var type = new CodeTypeDeclaration(className);
            defaultNamespace.Types.Add(type);
            return type;
        }

        public static CodeTypeDeclaration Interface(this CodeCompileUnit assembly, string interfaceName)
        {
            var defaultNamespace = new CodeNamespace(DefaultNamespaceName);
            assembly.Namespaces.Add(defaultNamespace);
            var type = new CodeTypeDeclaration(interfaceName);
            type.TypeAttributes = TypeAttributes.Public | TypeAttributes.Interface;
            defaultNamespace.Types.Add(type);
            return type;
        }
    }
}