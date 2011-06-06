using System.CodeDom;

namespace Refraction
{
    public static class CodeCompileUnitExtensions
    {
        public static CodeNamespace Namespace(this CodeCompileUnit assembly, string name)
        {
            var newNamespace = new CodeNamespace(name);
            assembly.Namespaces.Add(newNamespace);
            return newNamespace;
        }
    }
}
