using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
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
        private readonly ICodeBuffer _sourceCodeBuffer;
        private SourceText Code => SourceText.From(_sourceCodeBuffer.Code);
        
        private readonly CSharpCompilationOptions _compilationOptions = new(
            OutputKind.ConsoleApplication, 
            optimizationLevel: OptimizationLevel.Debug,
            assemblyIdentityComparer: DesktopAssemblyIdentityComparer.Default); 
        
        private readonly CSharpParseOptions _syntaxTreeOptions =
            CSharpParseOptions.Default.WithLanguageVersion(LanguageVersion.CSharp9);
        
        private readonly List<PortableExecutableReference> _references = 
            DependencyContext.Default.CompileLibraries
                .SelectMany(cl => cl.ResolveReferencePaths())
                .Select(asm => MetadataReference.CreateFromFile(asm))
                .ToList();

        public SourceCodeCompiler(ICodeBuffer code)
            => _sourceCodeBuffer = code;

        public void AddReference(string newReference)
            => _references.Add(MetadataReference.CreateFromFile(newReference));

        public byte[] Compile()
        {
            var compilation = CSharpCompilation.Create(
                assemblyName: "Doocutor.dll",
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
            OutputColorizer.colorizeForeground(ConsoleColor.Red, () => Console.WriteLine(message));
            throw new SourceCodeCompilationException(message);
        }
        
        private string GetMessageForCompilationResult(EmitResult result)
            => "Error of compilation:\n" + string.Join("\n", result.Diagnostics);

        private SyntaxTree GetSyntaxTree() => SyntaxFactory.ParseSyntaxTree(Code, _syntaxTreeOptions);
    }
}