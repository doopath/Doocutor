using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Text;
using Microsoft.CodeAnalysis.Emit;
using Microsoft.Extensions.DependencyModel;
using Doocutor.Core.CodeBuffers;
using Doocutor.Core.Exceptions;
using Libraries.Core;


namespace Doocutor.Core.CodeCompilers
{
    internal class SourceCodeCompiler : ICodeCompiler
    {
        private ICodeBuffer _codeBuffer;
        private SourceText _code;

        private readonly CSharpCompilationOptions _compilationOptions = new(
            OutputKind.ConsoleApplication, 
            optimizationLevel: OptimizationLevel.Release,
            assemblyIdentityComparer: DesktopAssemblyIdentityComparer.Default); 

        private readonly CSharpParseOptions _syntaxTreeOptions =
            CSharpParseOptions.Default.WithLanguageVersion(LanguageVersion.CSharp9);

        private readonly List<PortableExecutableReference> _references = 
            DependencyContext.Default.CompileLibraries
                .SelectMany(cl => cl.ResolveReferencePaths())
                .Select(asm => MetadataReference.CreateFromFile(asm))
                .ToList();

        public SourceCodeCompiler(ICodeBuffer codeBuffer)
        {
            _codeBuffer = codeBuffer;
        }

        public void AddReference(string newReference)
            => _references.Add(MetadataReference.CreateFromFile(newReference));

        public byte[] Compile()
        {
            _code = SourceText.From(_codeBuffer.Code);
            
            AddAssemblyInfo();
            
            var compilation = CSharpCompilation.Create(
                assemblyName: "MyLib.dll",
                syntaxTrees: new[] { GetSyntaxTree() },
                references: _references,
                options: _compilationOptions);
            using var peStream = new MemoryStream();
            var result = compilation.Emit(peStream);

            ThrowAnExceptionAndShowDiagnosticsIfFailed(result);
            peStream.Seek(0, SeekOrigin.Begin);

            return peStream.ToArray();
        }

        private void ThrowAnExceptionAndShowDiagnosticsIfFailed(EmitResult result)
        {
            if (result.Success) return;

            var message = GetMessageForCompilationResult(result);
            OutputColorizing.colorizeForeground(ConsoleColor.Red, () => Console.WriteLine(message));
            throw new SourceCodeCompilationException(message);
        }
        
        private string GetMessageForCompilationResult(EmitResult result)
            => "Error of compilation:\n" + string.Join("\n", result.Diagnostics);

        private SyntaxTree GetSyntaxTree()
        {
            return SyntaxFactory.ParseSyntaxTree(_code, _syntaxTreeOptions);   
        }

        private void AddAssemblyInfo()
        {
            var sourceCodeBuilder = new SourceCodeBuilder(_code.ToString());
            
            sourceCodeBuilder.AddUsing("using System.Reflection;"); 
            sourceCodeBuilder.AddAssemblySetting("[assembly: AssemblyTitle(\"MyLib\")]");
            sourceCodeBuilder.AddAssemblySetting("[assembly: AssemblyVersion(\"1.0.0\")]");

            _code = sourceCodeBuilder.GetSourceCode();
        }

    }

    internal class SourceCodeBuilder
    {
        private readonly string _code;
        private readonly StringBuilder _assemblySettings;
        private readonly StringBuilder _additionalUsings;
        private int _countOfUsingsInCode;
        private string[] _splitCode;

        public SourceCodeBuilder(string code)
        {
            _code = code;
            _assemblySettings = new StringBuilder();
            _additionalUsings = new StringBuilder();
            _countOfUsingsInCode = CountOfUsings(_code);
            _splitCode = _code.Trim().Split(";");
        }

        public void AddAssemblySetting(string setting)
        {
            RequireUniqueAssemblySetting(setting);
            _assemblySettings.AppendLine(setting);   
        }

        public void AddUsing(string additionalUsing)
        {
            if (!ContainsInAdditionalUsings(additionalUsing))
                _additionalUsings.AppendLine(additionalUsing);
        }

        public SourceText GetSourceCode()
            => SourceText.From(
                new StringBuilder()
                    .Append(_additionalUsings)
                    .Append(GetCodeUsings())
                    .Append(_assemblySettings)
                    .Append(GetCode())
                    .ToString());

        private string GetCodeUsings()
            => string.Join(";", _splitCode[.._countOfUsingsInCode]) + 
               (_countOfUsingsInCode > 0 ? ";\n\n" : "\n");
        
        private string GetCode()
            => string.Join(";", _splitCode[_countOfUsingsInCode..]);

        private bool ContainsInAdditionalUsings(string additionalUsing)
            => _additionalUsings.ToString().Split("\n").Contains(additionalUsing);

        private void RequireUniqueAssemblySetting(string setting)
        {
            if (_assemblySettings.ToString().Contains(setting))
                throw new Exception(
                    "(SourceCodeBuilder): Internal error: Someone tried to add an assembly that has been already added!");
        }

        private int CountOfUsings(string code)
            => CountOfUsings(code, 0);

        private int CountOfUsings(string code, int acc)
            => code.Trim().StartsWith("using")
                ? CountOfUsings(SkipFirstLine(code), acc+1)
                : acc;

        private string SkipFirstLine(string code)
            => string.Join(";", code.Trim().Split(";")[1..]);
    }
}