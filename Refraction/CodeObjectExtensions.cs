using System;
using System.CodeDom;
using System.Collections.Generic;

namespace Refraction
{
    public static class CodeObjectExtensions
    {
        public static void AddReferencedType<TReference>(this CodeObject codeObject)
        {
            codeObject.AddReferencedType(typeof(TReference));
        }

        public static void AddReferencedType(this CodeObject codeObject, Type type)
        {
            if (!codeObject.UserData.Contains("referencedTypes"))
            {
                codeObject.UserData["referencedTypes"] = new List<Type>();
            }
            ((List<Type>)codeObject.UserData["referencedTypes"]).Add(type);
        }

        public static IEnumerable<Type> GetReferencedTypes(this CodeObject codeObject)
        {
            if (codeObject.UserData.Contains("referencedTypes"))
            {
                return (List<Type>)codeObject.UserData["referencedTypes"];
            }

            return new List<Type>();
        }
    }
}
