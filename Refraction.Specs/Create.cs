using System.CodeDom;
using System.Reflection;
using Machine.Specifications;

namespace Refraction.Specs
{
    [Subject(typeof(Create))]
    public class when_adding_class_to_assembly_without_specifying_namespace
    {
        static Assembly assembly;
        static string className = "ClassName";

        Establish context = () =>
            {
                assembly = Create.Assembly(with => with.Class(className));
            };

        It should_add_the_class_to_a_the_default_namespace
            = () => assembly.GetTypeNamed(className).Namespace.ShouldEqual(Create.DefaultNamespaceName);
    }

    [Subject(typeof(Create))]
    public class when_adding_class_to_namespace_in_assembly
    {
        static Assembly assembly;
        static string namespaceName = "NamespaceName";
        static string className = "ClassName";

        Establish context = () =>
        {
            assembly = Create.Assembly(with =>
                {
                    with.Namespace(namespaceName)
                        .Class(className);
                });
        };

        It should_create_an_assembly_with_a_class_in_the_specified_namespace
            = () => assembly.GetTypeNamed(className).Namespace.ShouldEqual(namespaceName);
    }

    
}
