using System;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading;
using Machine.Specifications;

namespace Refraction.Specs.AutoImplementing
{
    [Subject(typeof(CodeTypeDeclarationExtensions), "AutoImplementing")]
    public abstract class AutoImplementingInstance
    {
        static Assembly assembly;
        static string className = "ClassName";
        protected static object instance;

        Establish context = () =>
        {
            assembly = Create.Assembly(with =>
                    with.Class(className)
                        .AutoImplementing<IDisposable>());

            instance = assembly.GetTypeInstance(className);
        };
    }

    public class instance_of_generated_class_autoimplementing_IDisposable : AutoImplementingInstance
    {
        It should_be_assignable_to_IDisposable
            = () => instance.ShouldBeOfType<IDisposable>();
    }

    public class invoking_Dispose_on_instance_of_generated_class_autoimplementing_IDisposable : AutoImplementingInstance
    {
        static Exception invokationException;
        
        Because of
            = () =>
            invokationException = Catch.Exception(() => instance.InvokeMember("Dispose", BindingFlags.InvokeMethod));

        It should_throw_NotImplementedException
            = () => invokationException.InnerException.ShouldBeOfType<NotImplementedException>();
    }

}
