using System;
using System.CodeDom;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace Refraction
{
    public static class CodeTypeDeclarationExtensions
    {
        public static CodeTypeDeclaration Abstract(this CodeTypeDeclaration type)
        {
            type.TypeAttributes = type.TypeAttributes | TypeAttributes.Abstract;
            return type;
        }

        public static CodeTypeDeclaration Inheriting<TBase>(this CodeTypeDeclaration type)
        {
            type.BaseTypes.Add(typeof(TBase));

            type.AddReferencedType<TBase>();

            return type;
        }

        public static CodeTypeDeclaration Inheriting(this CodeTypeDeclaration type, string baseClassName)
        {
            type.BaseTypes.Add(baseClassName);
            return type;
        }

        public static CodeTypeDeclaration Implementing<TInterface>(this CodeTypeDeclaration type)
        {
            type.BaseTypes.Add(typeof(TInterface));

            type.AddReferencedType<TInterface>();

            return type;
        }

        public static CodeTypeDeclaration AutoImplementing<TInterface>(this CodeTypeDeclaration type)
        {
            return type.AutoImplementing<TInterface>(x => { });
        }

        public static CodeTypeDeclaration AutoImplementing<TInterface>(this CodeTypeDeclaration type, Action<BaseImplementation<TInterface>> implementations)
        {
            type.BaseTypes.Add(typeof(TInterface));

            type.AddReferencedType<TInterface>();
            
            var baseImplementation = new BaseImplementation<TInterface>(type);
            implementations(baseImplementation);

            AddDefaultImplementations(typeof(TInterface), type);
            typeof(TInterface).GetBaseTypes().ToList()
                .ForEach(x => 
                {
                    type.AddReferencedType(x);
                    AddDefaultImplementations(x, type);
                });

            return type;
        }

        static void AddDefaultImplementations(Type baseType, CodeTypeDeclaration type)
        {
            AutoImplementMethods(baseType, type);
            AutoImplementProperties(baseType, type);
        }

        static bool NameAndParametersMatch(this CodeMemberMethod memberMethod, MethodInfo methodInfo)
        {
            if(!memberMethod.Name.Equals(methodInfo.Name))
            {
                return false;
            }

            if(memberMethod.Parameters.Count != methodInfo.GetParameters().Count())
            {
                return false;
            }

            for (int i = 0; i < memberMethod.Parameters.Count; i++)
            {
                if(memberMethod.Parameters[i].Type.BaseType.Equals(methodInfo.GetParameters()[i].ParameterType.FullName))
                {
                    return false;
                }
            }

            return true;
        }

        static bool ReturnTypesMatch(this CodeMemberMethod memberMethod, MethodInfo methodInfo)
        {
            return memberMethod.ReturnType.BaseType.Equals(methodInfo.ReturnType.FullName);
        }

        static void AutoImplementMethods(Type baseType, CodeTypeDeclaration type)
        {
            foreach (var methodInfo in baseType.GetMethods().Where(x => !x.Name.StartsWith("get_") && !x.Name.StartsWith("set_")))
            {
                var implementExplicit = false;
                if (type.Members.OfType<CodeMemberMethod>().Any(x => x.NameAndParametersMatch(methodInfo)))
                {
                    if (type.Members.OfType<CodeMemberMethod>().Any(x => x.ReturnTypesMatch(methodInfo)))
                    {
                        continue;
                    }
                    implementExplicit = true;
                }
                type.PublicMethod(x =>
                    {
                        if (implementExplicit)
                        {
                            x.PrivateImplementationType = new CodeTypeReference(baseType);
                        }
                        x.Named(methodInfo.Name);
                        x.Statements.Add(
                            new CodeThrowExceptionStatement(
                                new CodeObjectCreateExpression(typeof (NotImplementedException), new CodeExpression[0])));
                        x.ReturnType = new CodeTypeReference(methodInfo.ReturnType);
                        foreach (var parameterInfo in methodInfo.GetParameters())
                        {
                            x.Parameter(parameterInfo.ParameterType, parameterInfo.Name);
                        }
                    });
            }
        }

        static void AutoImplementProperties(Type baseType, CodeTypeDeclaration type)
        {
            foreach (var propertyInfo in baseType.GetProperties())
            {
                if (type.Members.OfType<CodeMemberProperty>().Any(x => x.Name.Equals(propertyInfo.Name)))
                {
                    continue;
                }

                type.Property(propertyInfo.PropertyType, x =>
                {
                    x.Named(propertyInfo.Name)
                        .GetStatements.Add(new CodeThrowExceptionStatement(
                                               new CodeObjectCreateExpression(typeof(NotImplementedException),
                                                                              new CodeExpression[0])));
                    if(propertyInfo.GetSetMethod().IsNotNull())
                    {
                        x.SetStatements.Add(new CodeThrowExceptionStatement(
                                            new CodeObjectCreateExpression(typeof (NotImplementedException),
                                                                           new CodeExpression[0])));
                    }
                    foreach (var parameterInfo in propertyInfo.GetIndexParameters())
                    {
                        x.Parameters.Add(new CodeParameterDeclarationExpression(parameterInfo.ParameterType, parameterInfo.Name));
                    }
                });
            }
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

        public static CodeTypeDeclaration Property<TProperty>(this CodeTypeDeclaration type, Action<CodeMemberProperty> propertyExpression)
        {
            return type.Property(typeof (TProperty), propertyExpression);
        }

        public static CodeTypeDeclaration Property(this CodeTypeDeclaration type, Type propertyType, Action<CodeMemberProperty> propertyExpression)
        {
            var property = new CodeMemberProperty();
            property.Type = new CodeTypeReference(propertyType);
            property.Attributes = MemberAttributes.Public;
            propertyExpression(property);
            type.Members.Add(property);
            return type;
        }

        public static CodeTypeDeclaration AutomaticProperty<TProperty>(this CodeTypeDeclaration type, string name)
        {
            type.AutomaticProperty<TProperty>(x =>
                {
                    x.Name = name;
                });

            return type;
        }

        public static CodeTypeDeclaration AutomaticProperty<TProperty>(this CodeTypeDeclaration type, Action<CodeMemberProperty> propertyExpression)
        {
            type.Property<TProperty>(property =>
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

        public static CodeTypeDeclaration VoidMethod(this CodeTypeDeclaration type, string name)
        {
            var method = new CodeMemberMethod();
            method.Name = name;
            method.Attributes = MemberAttributes.Public;
            type.Members.Add(method);
            return type;
        }

        public static CodeTypeDeclaration PublicMethod(this CodeTypeDeclaration type, Action<CodeMemberMethod> methodExpression)
        {
            var method = new CodeMemberMethod();
            method.Attributes = MemberAttributes.Public;
            methodExpression(method);
            type.Members.Add(method);
            return type;
        }

        public static CodeTypeDeclaration PublicMethod<TReturnType>(this CodeTypeDeclaration type, Action<NonVoidMethodDefinition<TReturnType>> methodExpression)
        {
            var method = new NonVoidMethodDefinition<TReturnType>();
            method.Attributes = MemberAttributes.Public;
            method.ReturnType = new CodeTypeReference(typeof(TReturnType));
            methodExpression(method);
            type.Members.Add(method);
            return type;
        }

        public static CodeTypeDeclaration PrivateField<TField>(this CodeTypeDeclaration type, Action<CodeMemberField> fieldExpression)
        {
            var field = new CodeMemberField();
            field.Type = new CodeTypeReference(typeof(TField));
            field.Attributes = MemberAttributes.Private;
            fieldExpression(field);
            type.Members.Add(field);
            return type;
        }
    }
}