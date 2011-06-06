using System.CodeDom;
using System.Reflection;

namespace Refraction
{
    public static class CodeMemberPropertyExtensions
    {
        public static CodeMemberProperty AnnotatedWith<TAttribute>(this CodeMemberProperty property)
        {
            property.AnnotatedWith<TAttribute>(new object());
            return property;
        }

        public static CodeMemberProperty AnnotatedWith<TAttribute>(this CodeMemberProperty property, object parameters, params object[] constructorParams)
        {
            var ctorParams = new CodeAttributeArgument[constructorParams.Length];
            for (int i = 0; i < constructorParams.Length; i++)
            {
                ctorParams[i] = new CodeAttributeArgument(null, new CodePrimitiveExpression(constructorParams[i]));
            }
            var attribute = new CodeAttributeDeclaration(new CodeTypeReference(typeof (TAttribute)), ctorParams);
            foreach (var propertyInfo in parameters.GetType().GetProperties())
            {
                var parameterValue = parameters.GetType().InvokeMember(propertyInfo.Name, BindingFlags.GetProperty, null, parameters, null);
                var parameterName = propertyInfo.Name;
                attribute.Arguments.Add(new CodeAttributeArgument(parameterName,
                                                                  new CodePrimitiveExpression(parameterValue)));
            }
            
            property.CustomAttributes.Add(attribute);
            property.AddReferencedType<TAttribute>();
            return property;
        }

        public static CodeMemberProperty Named(this CodeMemberProperty property, string name)
        {
            property.Name = name;
            return property;
        }

        public static CodeMemberProperty IsOverride(this CodeMemberProperty property)
        {
            property.Attributes = property.Attributes | MemberAttributes.Override;
            return property;
        }

        public static CodeMemberProperty Static(this CodeMemberProperty property)
        {
            property.Attributes = property.Attributes | MemberAttributes.Static;
            return property;
        }
    }
}