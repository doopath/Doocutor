using System;
using System.IO;
using System.Runtime.Loader;
using Doocutor.Core.Exceptions;

namespace Doocutor.Core
{
    internal class CompiledCodeExecutor
    {
        
        public void Execute(byte[] code, string[] args)
        {
            using var asm = new MemoryStream(code);
            
            var assemblyLoadContext = new AssemblyLoadContext("Doocutor", true);
            var assembly = assemblyLoadContext.LoadFromStream(asm);
            var entry = assembly.EntryPoint;

            if (entry is null)
                throw new CompiledCodeExecutionException("Cannot find entry point in you code!");
            
            ShowOutputMessage();
            
            _ = entry.GetParameters().Length > 0
                ? entry.Invoke(null, new object[] {args})
                : entry.Invoke(null, null);
            
            assemblyLoadContext.Unload();
        }
        
        private void ShowOutputMessage()
            => OutputColorizer.ColorizeForeground(ConsoleColor.Blue, () => Console.WriteLine("Output:"));
    }
}