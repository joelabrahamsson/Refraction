using System;
using System.CodeDom;
using System.Linq.Expressions;

namespace Refraction
{
    public static class CodeMemberMethodExtensions
    {
        public static CodeMemberMethod Abstract(this CodeMemberMethod method)
        {
            method.Attributes = method.Attributes | MemberAttributes.Abstract;
            return method;
        }

        public static CodeMemberMethod IsOverride(this CodeMemberMethod method)
        {
            method.Attributes = method.Attributes | MemberAttributes.Override;
            return method;
        }

        public static TMethod Named<TMethod>(this TMethod method, string name)
            where TMethod : CodeMemberMethod
        {
            method.Name = name;
            return method;
        }

        public static CodeMemberMethod Parameter<TParameter>(this CodeMemberMethod method, string name)
        {
            return method.Parameter(typeof (TParameter), name);
        }

        public static CodeMemberMethod Parameter(this CodeMemberMethod method, Type parameterType, string name)
        {
            method.Parameters.Add(new CodeParameterDeclarationExpression(parameterType, name));
            return method;
        }

        public static CodeMemberMethod Body(this CodeMemberMethod method, string methodBody)
        {
            method.Statements.Add(new CodeSnippetExpression(methodBody));
            return method;
        }

        public static NonVoidMethodDefinition<TReturnType> ReturnValue<TReturnType>(this NonVoidMethodDefinition<TReturnType> method, TReturnType value)
        {
            method.Statements.Add(new CodeMethodReturnStatement(new CodePrimitiveExpression(value)));
            return method;
        }

        public static CodeMemberMethod Body(this CodeMemberMethod method, string format, params object[] args)
        {
            return method.Body(string.Format(format, args));
        }
    }
}