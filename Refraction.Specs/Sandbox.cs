using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Machine.Specifications;
using PageTypeBuilder;
using PageTypeBuilder.Migrations;
using PageTypeBuilder.Synchronization.Hooks;

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
                    .Inheriting<TypedPageData>()
                    .Implementing<IDisposable>()
                    .AnnotatedWith<PageTypeAttribute>(new { Name = "My page type" })
                    .WithVoidMethod("Dispose")
                    .WithMethod(x =>
                    {
                        x.Name = "PreSynchronization";
                        x.Parameter<ISynchronizationHookContext>("context");
                    })
                    .Implementing<IPreSynchronizationHook>()
                    .WithAutomaticProperty<string>(x =>
                    {
                        x.Name = "MainBody";
                        x.AnnotatedWith<PageTypePropertyAttribute>();
                    });
            });

            assembly2 = Create.Assembly(with =>
            {
                with.Class("MyClass")
                    .Inheriting<Migration>()
                    .WithAutomaticProperty<bool>(x =>
                    {
                        x.Named("ExecuteHasBeenCalled");
                        x.Static();
                    })
                    .WithMethod(x =>
                    {
                        x.Named("Execute")
                            .IsOverride()
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
            assembly.GetTypes().First().BaseType.ShouldEqual(typeof(TypedPageData));

        It should_create_a_class_annotated
            =
            () =>
            assembly.GetTypes().First().GetCustomAttributes(typeof(PageTypeAttribute), false).ShouldNotBeEmpty();

        It should_create_a_class_annotated_with_property_set
            =
            () =>
            ((PageTypeAttribute)
             assembly.GetTypes().First().GetCustomAttributes(typeof(PageTypeAttribute), false).First()).Name.
                ShouldEqual("My page type");

        It should_create_property
            =
            () =>
            assembly.GetTypes().First().GetMember("MainBody").ShouldNotBeEmpty();

        It should_create_property_with_annotation
            =
            () =>
            assembly.GetTypes().First().GetMember("MainBody").First().GetCustomAttributes(
                typeof(PageTypePropertyAttribute), false).ShouldNotBeEmpty();
    }
}
