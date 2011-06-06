using System.CodeDom;

namespace Refraction
{
    public static class CodeMemberMethodExtensions
    {
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
            method.Parameters.Add(new CodeParameterDeclarationExpression(typeof(TParameter), name));
            return method;
        }

        public static CodeMemberMethod Body(this CodeMemberMethod method, string methodBody)
        {
            method.Statements.Add(new CodeSnippetExpression(methodBody));
            return method;
        }
    }
}