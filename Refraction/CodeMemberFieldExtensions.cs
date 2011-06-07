using System.CodeDom;

namespace Refraction
{
    public static class CodeMemberFieldExtensions
    {
        public static CodeMemberField Named(this CodeMemberField field, string name)
        {
            field.Name = name;
            return field;
        }
    }
}
