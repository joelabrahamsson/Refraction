using System;
using System.CodeDom;

namespace Refraction
{
    public static class CodeConstructorExtensions
    {
        public static CodeConstructor Parameter<TParameter>(this CodeConstructor constructor, string name)
        {
            return constructor.Parameter(typeof(TParameter), name);
        }

        public static CodeConstructor Parameter(this CodeConstructor constructor, Type parameterType, string name)
        {
            constructor.Parameters.Add(new CodeParameterDeclarationExpression(parameterType, name));
            return constructor;
        }

        public static CodeConstructor PassToBase(this CodeConstructor constructor, string parameterName)
        {
            constructor.BaseConstructorArgs.Add(new CodeArgumentReferenceExpression(parameterName));
            return constructor;
        }
    }
}
