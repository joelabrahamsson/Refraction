using System;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using Microsoft.CSharp;

namespace Refraction
{
    public class AssemblyDefinition : CodeCompileUnit 
    {
        public const string DefaultNamespace = "DefaultNamespace";

        public AssemblyDefinition()
        {
            CompilerParameters = new CompilerParameters();
            CompilerParameters.GenerateExecutable = false;
            CompilerParameters.GenerateInMemory = true;
        }

        public string Name
        {
            get
            {
                return CompilerParameters.OutputAssembly;
            }
            set
            {
                CompilerParameters.OutputAssembly = value;
            }
        }

        public CompilerParameters CompilerParameters { get; private set; }

        public void AddAssemblyReference(string assemblyName)
        {
            CompilerParameters.ReferencedAssemblies.Add(assemblyName);
        }

        void AddAssemblyReference(Type type)
        {
            ReferencedAssemblies.Add(type.Assembly.GetName().Name + ".dll");
            foreach (var baseType in type.GetBaseTypes())
            {
                ReferencedAssemblies.Add(baseType.Assembly.GetName().Name + ".dll");
            }
            foreach (var referencedAssembly in type.Assembly.GetReferencedAssemblies())
            {
                ReferencedAssemblies.Add(referencedAssembly.Name + ".dll");
            }
        }

        public Assembly BuildAssembly()
        {
            GetReferencedTypes().ForEach(AddAssemblyReference);

            CSharpCodeProvider provider =
              new CSharpCodeProvider();
            ICodeCompiler compiler = provider.CreateCompiler();
            CompilerResults results =
               compiler.CompileAssemblyFromDom(CompilerParameters, this);
            if (results.Errors.HasErrors)
            {
                StringBuilder errors = new StringBuilder("Compiler Errors :\r\n");
                foreach (CompilerError error in results.Errors)
                {
                    errors.AppendFormat("Line {0},{1}\t: {2}\n",
                           error.Line, error.Column, error.ErrorText);
                }
                var buffer = new StringBuilder();
                using(var writer = new StringWriter(buffer))
                {
                    using (var textWriter = new IndentedTextWriter(writer))
                    {
                        provider.GenerateCodeFromCompileUnit(this, textWriter, new CodeGeneratorOptions());
                        textWriter.Close();
                    }
                }

                var source = new StringBuilder();
                var lines = buffer.ToString().Split(new string[] {Environment.NewLine}, StringSplitOptions.None);
                for(int i = 0; i < lines.Length; i++)
                {
                    source.AppendLine(i + lines[i]);
                }
                throw new Exception(
                    errors.ToString() 
                    + Environment.NewLine
                    + Environment.NewLine
                    + "CODE:"
                    + Environment.NewLine
                    + source);
            }

            return results.CompiledAssembly;
        }

        private List<Type> GetReferencedTypes()
        {
            var referencedTypes = new List<Type>();
            foreach (CodeNamespace ns in Namespaces)
            {
                foreach (CodeTypeDeclaration type in ns.Types)
                {
                    foreach (var referencedType in type.GetReferencedTypes())
                    {
                        referencedTypes.Add(referencedType);
                    }

                    foreach (CodeTypeMember member in type.Members)
                    {
                        foreach (var referencedType in member.GetReferencedTypes())
                        {
                            referencedTypes.Add(referencedType);
                        }
                    }
                }
            }
            return referencedTypes.Distinct().ToList();
        }
    }
}
