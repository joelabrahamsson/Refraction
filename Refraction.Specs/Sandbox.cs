using System;
using System.CodeDom;
using System.Collections;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using Machine.Specifications;

namespace Refraction.Specs
{
    public class ExampleAttribute : Attribute
    {
        public ExampleAttribute(Type type)
        {
            Type = type;
        }

        public Type Type { get; set; }

        public Type Type2 { get; set; }
    }

    public class property_annotated_with_attribute_refering_to_generated_type
    {
        static Assembly assembly;
        static string className = "ClassName";
        static string propertyName = "MyProperty";

        Establish context = () =>
            {
                assembly = Create.Assembly(with => {
                    var referenced = with.Class("Sample");

                    with.Class(className)
                    .AutomaticProperty<string>(x =>
                    x.Named(propertyName)
                    .AnnotatedWith<ExampleAttribute>(new { Type2 = referenced }, referenced));
                });
            };

        It should =
            () =>
            assembly.GetTypeNamed(className).GetProperty(propertyName).GetCustomAttributes(typeof (ExampleAttribute),
                                                                                           false).ShouldNotBeNull();
    }

    //TODO: Test non-primitive values, or add restrictions
    [Subject(typeof(CodeMemberMethodExtensions), "ReturnValue")]
    public class invoking_method_on_type_with_method_returning_string_constant
    {
        static Assembly assembly;
        static string className = "ClassName";
        static string methodName = "SayHi";
        static string returnValue = Guid.NewGuid().ToString();

        Establish context = () =>
        {
            assembly = Create.Assembly(with =>
                    with.Class(className)
                    .PublicMethod<string>(x => 
                        x.Named(methodName)
                         .ReturnValue(returnValue)));
        };

        It should_return_the_string_constant
            = () =>
              assembly.GetTypeInstance(className).InvokeMember<string>(methodName, BindingFlags.InvokeMethod, new object[0]).
                  ShouldEqual(returnValue);
              
    }

    public class when_created_type_with_constructor_with_two_parameters
    {
        static Assembly assembly;
        static string className = "ClassName";

        Establish context = () =>
        {
            assembly = Create.Assembly(with =>
                with.Class(className)
                .Constructor(x =>
                    {
                        x.Parameter<string>("stringParam")
                            .Parameter<int>("intParam");
                    }));
        };

        It should_be_possible_to_instantiate_it_by_passing_two_parameters_to_the_constructor
            = () =>
              assembly.GetTypeInstance(className, "", 1).ShouldNotBeNull();
    }

    public class when_created_type_inherits_type_with_constructor_parameter
    {
        static Assembly assembly;
        static string className = "ClassName";

        Establish context = () =>
        {
            assembly = Create.Assembly(with =>
                {
                    with.Class("baseClass")
                        .Constructor(x =>
                            {
                                x.Parameter<string>("stringParam");
                            });

                    with.Class(className)
                        .Inheriting("baseClass")
                        .Constructor(x =>
                            {
                                x.Parameter<string>("stringParam")
                                    .PassToBase("stringParam");
                            });
                });
        };

        It should_be_possible_to_instantiate_it_by_passing_two_parameters_to_the_constructor
            = () =>
              assembly.GetTypeInstance(className, "").ShouldNotBeNull();
    }

    [Subject(typeof(CodeTypeDeclarationExtensions), "Abstract")]
    public class when_invoked_on_a_CodeTypeDeclaration
    {
        static Assembly assembly;
        static string className = "ClassName";

        Establish context = () =>
        {
            assembly = Create.Assembly(with =>
                with.Class(className)
                    .Abstract());
        };

        It should_create_abstract_class
            = () =>
              assembly.GetTypeNamed(className).IsAbstract.ShouldBeTrue();
    }

    public class abstract_method
    {
        static Assembly assembly;
        static string className = "ClassName";
        static string methodName = "MethodName";

        Establish context = () =>
        {
            assembly = Create.Assembly(with =>
                with.Class(className)
                    .Abstract()
                    .PublicMethod(x =>
                    x.Named(methodName)
                     .Abstract()));
        };

        It should_create_abstract_method
            = () =>
              assembly.GetTypeNamed(className).GetMethod(methodName).IsAbstract.ShouldBeTrue();
    }

    public class abstract_property
    {
        static Assembly assembly;
        static string className = "ClassName";
        static string propertyName = "MethodName";

        Establish context = () =>
        {
            assembly = Create.Assembly(with =>
                with.Class(className)
                    .Abstract()
                    .AutomaticProperty<string>(x =>
                    x.Named(propertyName)
                     .Abstract()));
        };

        It should_create_property_with_abstract_getter
            = () =>
              assembly.GetTypeNamed(className).GetProperty(propertyName).GetGetMethod().IsAbstract.ShouldBeTrue();
    }

    public class automatic_interface_with_property_implementation
    {
        static Assembly assembly;
        static string className = "ClassName";
        static Exception thrownException;

        Establish context = () =>
        {
            thrownException = Catch.Exception(() =>
                assembly = Create.Assembly(with =>
                    with.Class(className)
                        .AutoImplementing<IDictionary>()));
        };

        It should_compile
            = () =>
              thrownException.ShouldBeNull();

        It should_create_class_implementing_the_interface
            = () => assembly.GetTypeNamed(className).GetInterfaces().ShouldContain(typeof(IDictionary));
    }

    public class given_property_with_getter_snippet_implementation
    {
        static Assembly assembly;
        static string className = "ClassName";
        static string propertyName = "PropertyName";
        static string expected = "Hey baberiba";
        
        Establish context = () =>
        {
            assembly = Create.Assembly(with => 
                with.Class(className)
                    .Property<string>(x => 
                        x.Named(propertyName)
                         .GetterBody("return \"{0}\";", expected)));        
        };

        It should_create_property_returning_expected_value
            = () => 
                assembly.GetTypeInstance(className)
                .InvokeMember<string>(propertyName, BindingFlags.GetProperty)
                        .ShouldEqual(expected);
    }

    public class property_with_backing_field
    {
        static Assembly assembly;
        static string className = "ClassName";
        static string propertyName = "PropertyName";
        static string fieldName = "myField";
        static string expected = "Hey baberiba";
        static string actual;
        
        Establish context = () =>
        {
            assembly = Create.Assembly(with =>
            {
                with.Class(className)
                    .PrivateField<string>(x => x.Named(fieldName))
                    .Property<string>(x =>
                        {
                            x.Name = propertyName;
                            x.GetterBody(string.Format("return {0};", fieldName));
                            x.SetterBody(string.Format("{0} = value;", fieldName));
                        });
            });

            object instance = assembly.GetTypeInstance(className);
            instance.InvokeMember<string>(propertyName, BindingFlags.SetProperty, expected);
            actual = instance.InvokeMember<string>(propertyName, BindingFlags.GetProperty);
        };

        private It should_create_member_with_specified_name
            = () =>
              actual.ShouldEqual(expected);
    }

    public class method_with_parameter
    {
        static Assembly assembly;
        static string className = "ClassName";
        static string verificationPropertyName = "parameterValue";
        static string methodName = "methodName";
        static string expected = "Hey baberiba";
        static string actual;

        Establish context = () =>
        {
            assembly = Create.Assembly(with => 
                with.Class(className)
                    .AutomaticProperty<string>(verificationPropertyName)
                    .PublicMethod(x => 
                        x.Named(methodName)
                        .Parameter<string>("parameter")
                        .Body("{0} = {1};", verificationPropertyName, "parameter")));

            object instance = assembly.GetTypeInstance(className);
            instance.InvokeMember<string>(methodName, BindingFlags.InvokeMethod, expected);
            actual = instance.InvokeMember<string>(verificationPropertyName, BindingFlags.GetProperty);
        };

        private It should_create_member_with_specified_name
            = () =>
              actual.ShouldEqual(expected);
    }

    [Subject(typeof(AssemblyDefinition), "BuildAssembly method")]
    public class sandbox
    {
        static Assembly assembly;
        static Assembly assembly2;
        Establish context = () =>
        {
            assembly = Create.Assembly(with =>
            {
                with.Class("MyClass")
                    .Inheriting<NullReferenceException>()
                    .Implementing<IDisposable>()
                    .AnnotatedWith<DebuggerDisplayAttribute>(new { Name = "Some text" }, "value")
                    .VoidMethod("Dispose")
                    .AutomaticProperty<string>(x =>
                    {
                        x.Name = "MainBody";
                        x.AnnotatedWith<DebuggerDisplayAttribute>(new { Name = "Some text" }, "value");
                    });
            });

            assembly2 = Create.Assembly(with =>
            {
                with.Class("MyClass")
                    .AutomaticProperty<bool>(x =>
                    {
                        x.Named("ExecuteHasBeenCalled");
                        x.Static();
                    })
                    .PublicMethod(x =>
                    {
                        x.Named("Execute")
                            .Body("ExecuteHasBeenCalled = true;");
                    });
            });

            var type = assembly2.GetTypeNamed("MyClass");
            var instance = Activator.CreateInstance(type);
            type.InvokeMember("Execute", BindingFlags.InvokeMethod, null, instance, null);
        };

        It should_create_a_class2
            =
            () =>
            assembly2.GetTypeInstance("MyClass").InvokeMember<bool>("ExecuteHasBeenCalled", BindingFlags.GetProperty).
                ShouldBeTrue();

        It should_create_a_class
            =
            () =>
            assembly.GetTypes().ShouldNotBeEmpty();

        It should_create_a_class_named
            =
            () =>
            assembly.GetTypes().First().Name.ShouldEqual("MyClass");

        It should_create_a_class_with_base_class
            =
            () =>
            assembly.GetTypes().First().BaseType.ShouldEqual(typeof(NullReferenceException));

        It should_create_a_class_annotated
            =
            () =>
            assembly.GetTypes().First().GetCustomAttributes(typeof(DebuggerDisplayAttribute), false).ShouldNotBeEmpty();

        It should_create_a_class_annotated_with_property_set
            =
            () =>
            ((DebuggerDisplayAttribute)
             assembly.GetTypes().First().GetCustomAttributes(typeof(DebuggerDisplayAttribute), false).First()).Name.
                ShouldEqual("Some text");

        It should_create_property
            =
            () =>
            assembly.GetTypes().First().GetMember("MainBody").ShouldNotBeEmpty();

        It should_create_property_with_annotation
            =
            () =>
            assembly.GetTypes().First().GetMember("MainBody").First().GetCustomAttributes(
                typeof(DebuggerDisplayAttribute), false).ShouldNotBeEmpty();
    }
}
