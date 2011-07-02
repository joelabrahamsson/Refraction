using System;
using System.CodeDom;
using System.Reflection;

namespace Refraction
{
    public static class CodeMemberPropertyExtensions
    {
        public static CodeMemberProperty Abstract(this CodeMemberProperty property)
        {
            property.Attributes = property.Attributes | MemberAttributes.Abstract;
            return property;
        }

        public static CodeMemberProperty AnnotatedWith<TAttribute>(this CodeMemberProperty property)
        {
            property.AnnotatedWith<TAttribute>(new object());
            return property;
        }

        public static CodeMemberProperty AnnotatedWith<TAttribute>(this CodeMemberProperty property, object parameters, params object[] constructorParams)
        {
            property.AddReferencedType<TAttribute>();
            var attributeType = new CodeTypeReference(typeof (TAttribute));
            return property.AnnotatedWith(attributeType, parameters, constructorParams);
        }


        public static CodeMemberProperty AnnotatedWith(this CodeMemberProperty property, CodeTypeReference attributeType, object parameters, params object[] constructorParams)
        {
            CodeAttributeArgument[] ctorParams = GetCtorParams(constructorParams);
            var attribute = new CodeAttributeDeclaration(attributeType, ctorParams);
            foreach (var propertyInfo in parameters.GetType().GetProperties())
            {
                var value = parameters.GetType().InvokeMember(propertyInfo.Name, BindingFlags.GetProperty, null, parameters, null);
                CodeExpression parameterValue;
                if (value is Type)
                {
                    parameterValue = new CodeTypeOfExpression((Type)value);
                }
                else if (value is CodeTypeDeclaration)
                {
                    parameterValue = new CodeTypeOfExpression(((CodeTypeDeclaration)value).Name);
                }
                else if (value is Enum)
                {
                    parameterValue = new CodeCastExpression(propertyInfo.PropertyType, new CodePrimitiveExpression((int)value));
                }
                else
                {
                    parameterValue = new CodePrimitiveExpression(value);
                }
                
                var parameterName = propertyInfo.Name;
                attribute.Arguments.Add(new CodeAttributeArgument(parameterName, parameterValue));
            }
            
            property.CustomAttributes.Add(attribute);
            return property;
        }

        static CodeAttributeArgument[] GetCtorParams(object[] constructorParams)
        {
            var ctorParams = new CodeAttributeArgument[constructorParams.Length];
            for (int i = 0; i < constructorParams.Length; i++)
            {
                var ctorParam = constructorParams[i];
                if(ctorParam is Type)
                {
                    ctorParams[i] = new CodeAttributeArgument(null, new CodeTypeReferenceExpression((Type)ctorParam));
                }
                else if(ctorParam is CodeTypeDeclaration)
                {
                    ctorParams[i] = new CodeAttributeArgument(null, new CodeTypeOfExpression(((CodeTypeDeclaration) ctorParam).Name));
                }
                else
                {
                    ctorParams[i] = new CodeAttributeArgument(null, new CodePrimitiveExpression(constructorParams[i]));   
                }
            }
            return ctorParams;
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

        public static CodeMemberProperty GetterBody(this CodeMemberProperty property, string methodBody)
        {
            property.GetStatements.Add(new CodeSnippetExpression(methodBody));
            return property;
        }

        public static CodeMemberProperty GetterBody(this CodeMemberProperty property, string format, params object[] args)
        {
            return property.GetterBody(string.Format(format, args));
        }

        public static CodeMemberProperty Returning(this CodeMemberProperty property, string stringConstant)
        {
            if (stringConstant != null)
            {
                return property.GetterBody("return \"{0}\";", stringConstant);
            }

            return property.GetterBody("return null;");
        }

        public static CodeMemberProperty Returning(this CodeMemberProperty property, bool value)
        {
            return property.GetterBody("return {0};", value.ToString().ToLower());
        }

        public static CodeMemberProperty SetterBody(this CodeMemberProperty property, string methodBody)
        {
            property.SetStatements.Add(new CodeSnippetExpression(methodBody));
            return property;
        }

        public static CodeMemberProperty SetterBody(this CodeMemberProperty property, string format, params object[] args)
        {
            return property.SetterBody(string.Format(format, args));
        }
    }
}