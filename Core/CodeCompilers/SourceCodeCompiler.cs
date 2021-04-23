using System.IO;
using System.Linq;
using System.Collections.Generic;
using NLog;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Text;
using Microsoft.CodeAnalysis.Emit;
using Microsoft.Extensions.DependencyModel;
using Doocutor.Core.CodeBuffers;
using Doocutor.Core.Exceptions;

namespace Doocutor.Core.CodeCompilers
{
    internal class SourceCodeCompiler : ICodeCompiler
    {
        private readonly Logger _logger = LogManager.GetCurrentClassLogger();
        private readonly ICodeBuffer _sourceCodeBuffer;
        private SourceText Code => SourceText.From(_sourceCodeBuffer.Code);
        
        private readonly CSharpCompilationOptions _compilationOptions = new CSharpCompilationOptions(
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

            CheckIfFailed(result);

            peStream.Seek(0, SeekOrigin.Begin);

            return peStream.ToArray();
        }

        private void CheckIfFailed(EmitResult result)
        {
            if (result.Success) return;
            
            var message = "Error of compilation:\n" + string.Join("\n", result.Diagnostics);
                
            _logger.Error(message);
            throw new SourceCodeCompilationException(message);
        }

        private SyntaxTree GetSyntaxTree() => SyntaxFactory.ParseSyntaxTree(Code, _syntaxTreeOptions);
    }
}