using System;
using System.CodeDom;
using System.Linq.Expressions;

namespace Refraction
{
    public class BaseImplementation<TBase>
    {
        CodeTypeDeclaration type;
        public BaseImplementation(CodeTypeDeclaration type)
        {
            this.type = type;
        }

        public CodeMemberMethod DefineMethod(Expression<Action<TBase>> memberNameExpression)
        {
            var method = new CodeMemberMethod();
            method.Named(StaticReflection.GetMemberName(memberNameExpression));
            method.Attributes = MemberAttributes.Public;
            type.Members.Add(method);
            return method;
        }
    }
}