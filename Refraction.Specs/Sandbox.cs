using System;
using System.CodeDom.Compiler;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using Machine.Specifications;

namespace Refraction.Specs
{
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
                    .WithVoidMethod("Dispose")
                    .WithAutomaticProperty<string>(x =>
                    {
                        x.Name = "MainBody";
                        x.AnnotatedWith<DebuggerDisplayAttribute>(new { Name = "Some text" }, "value");
                    });
            });

            assembly2 = Create.Assembly(with =>
            {
                with.Class("MyClass")
                    .WithAutomaticProperty<bool>(x =>
                    {
                        x.Named("ExecuteHasBeenCalled");
                        x.Static();
                    })
                    .WithMethod(x =>
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
