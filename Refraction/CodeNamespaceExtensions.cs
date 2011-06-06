using System.CodeDom;

namespace Refraction
{
    public static class CodeNamespaceExtensions
    {
        public static CodeTypeDeclaration Class(this CodeNamespace codeNamespace, string name)
        {
            var newClass = new CodeTypeDeclaration(name);
            codeNamespace.Types.Add(newClass);
            return newClass;
        }
    }
}
