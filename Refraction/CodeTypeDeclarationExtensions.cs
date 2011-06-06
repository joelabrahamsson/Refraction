using System;
using System.CodeDom;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace Refraction
{
    public static class CodeTypeDeclarationExtensions
    {
        public static CodeTypeDeclaration Inheriting<TBase>(this CodeTypeDeclaration type)
        {
            type.BaseTypes.Add(typeof(TBase));

            type.AddReferencedType<TBase>();

            return type;
        }

        public static CodeTypeDeclaration Implementing<TInterface>(this CodeTypeDeclaration type)
        {
            type.BaseTypes.Add(typeof(TInterface));

            type.AddReferencedType<TInterface>();

            return type;
        }

        public static CodeTypeDeclaration AnnotatedWith<TAttribute>(this CodeTypeDeclaration type)
        {
            return type.AnnotatedWith<TAttribute>(new object());
        }

        public static CodeTypeDeclaration AnnotatedWith<TAttribute>(this CodeTypeDeclaration type, object parameters, params object[] constructorParams)
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
            
            type.CustomAttributes.Add(attribute);
            return type;
        }

        public static CodeTypeDeclaration WithProperty<TProperty>(this CodeTypeDeclaration type, Action<CodeMemberProperty> propertyExpression)
        {
            var property = new CodeMemberProperty();
            property.Type = new CodeTypeReference(typeof(TProperty));
            property.Attributes = MemberAttributes.Public;
            //type.AddAssemblyReference<TProperty>();
            propertyExpression(property);
            type.Members.Add(property);
            return type;
        }

        public static CodeTypeDeclaration WithAutomaticProperty<TProperty>(this CodeTypeDeclaration type, string name)
        {
            type.WithAutomaticProperty<TProperty>(x =>
                {
                    x.Name = name;
                });

            return type;
        }

        public static CodeTypeDeclaration WithAutomaticProperty<TProperty>(this CodeTypeDeclaration type, Action<CodeMemberProperty> propertyExpression)
        {
            type.WithProperty<TProperty>(property =>
                {
                    propertyExpression(property);

                    CodeMemberField backingField = CreateAutomaticBackingField<TProperty>(property);
                    type.Members.Add(backingField);

                    property.CustomAttributes.Add(
                        new CodeAttributeDeclaration(new CodeTypeReference(typeof(CompilerGeneratedAttribute))));
                    property.GetStatements.Add(
                        ReturnFieldStatement(type, backingField));

                    property.SetStatements.Add(
                        FieldAssignStatement(type, backingField, new CodeArgumentReferenceExpression("value")));
                });

            return type;
        }

        static CodeMemberField CreateAutomaticBackingField<TProperty>(CodeMemberProperty property)
        {
            string backingFieldName = GetBackingFieldName(property.Name);
            var backingField = new CodeMemberField(typeof(TProperty), backingFieldName);
            if (property.Attributes.HasFlag(MemberAttributes.Static))
            {
                backingField.Attributes = backingField.Attributes | MemberAttributes.Static;
            }
            return backingField;
        }

        static string GetBackingFieldName(string propertyName)
        {
            return string.Format("hereBeAngleBracket{0}hereBeAngleBracketk__BackingField", propertyName);
        }

        static CodeMethodReturnStatement ReturnFieldStatement(CodeTypeDeclaration type, CodeMemberField field)
        {
            CodeExpression referenceExpression;
            if(field.Attributes.HasFlag(MemberAttributes.Static))
            {
                referenceExpression = new CodeTypeReferenceExpression(type.Name);
            }
            else
            {
                referenceExpression = new CodeThisReferenceExpression();;   
            }
            
            var fieldReference = new CodeFieldReferenceExpression(
                referenceExpression, field.Name);
            return new CodeMethodReturnStatement(fieldReference);
        }

        static CodeAssignStatement FieldAssignStatement(CodeTypeDeclaration type, CodeMemberField field, CodeExpression valueStatement)
        {
            CodeExpression referenceExpression;
            if (field.Attributes.HasFlag(MemberAttributes.Static))
            {
                referenceExpression = new CodeTypeReferenceExpression(type.Name);
            }
            else
            {
                referenceExpression = new CodeThisReferenceExpression(); ;
            }

            var fieldReference = new CodeFieldReferenceExpression(
                referenceExpression, field.Name);
            return new CodeAssignStatement(fieldReference, valueStatement);
        }

        public static CodeTypeDeclaration WithVoidMethod(this CodeTypeDeclaration type, string name)
        {
            var method = new CodeMemberMethod();
            method.Name = name;
            method.Attributes = MemberAttributes.Public;
            type.Members.Add(method);
            return type;
        }

        public static CodeTypeDeclaration WithMethod(this CodeTypeDeclaration type, Action<CodeMemberMethod> methodExpression)
        {
            var method = new CodeMemberMethod();
            method.Attributes = MemberAttributes.Public;
            methodExpression(method);
            type.Members.Add(method);
            return type;
        }
    }
}