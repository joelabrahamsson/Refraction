using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Refraction
{
    public class NamespaceDefinition : CodeNamespace
    {
        public NamespaceDefinition() {}
        public NamespaceDefinition(string name)
            : base(name)
        {}
    }
}
