using System;
using System.CodeDom;

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

        public static CodeMemberMethod Named(this CodeMemberMethod method, string name)
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

        public static CodeMemberMethod Body(this CodeMemberMethod method, string format, params object[] args)
        {
            return method.Body(string.Format(format, args));
        }
    }
}