using System;
using System.CodeDom;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Refraction
{
    public static class CollectionExtensions
    {
        public static IEnumerable<CodeNamespace> GetNamespaces(this CodeNamespaceCollection collection)
        {
            foreach (CodeNamespace codeNamespace in collection)
            {
                yield return codeNamespace;
            }
        }

        public static IEnumerable<CodeTypeDeclaration> GetTypes(this CodeTypeDeclarationCollection collection)
        {
            foreach (CodeTypeDeclaration type in collection)
            {
                yield return type;
            }
        }
    }
}
