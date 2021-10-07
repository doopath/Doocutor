using System;
using System.IO;
using System.Reflection;
using System.Runtime.Loader;
using Domain.Core.Exceptions;
using Libraries.Core;

namespace Domain.Core
{
    public class CompiledCodeExecutor
    {
        
        public void Execute(byte[] code, string[] args)
        {
            using var asm = new MemoryStream(code);
            
            var assemblyLoadContext = new AssemblyLoadContext("Doocutor", true);
            var assembly = assemblyLoadContext.LoadFromStream(asm);
            var entry = assembly.EntryPoint;
            
            if (entry is null)
                throw new CompiledCodeExecutionException("Cannot find entry point in your code!");

            ShowOutput(entry, args);
            
            assemblyLoadContext.Unload();
        }

        private void ShowOutput(MethodInfo entry, string[] args)
        {
            ShowOutputMessage();
            entry.Invoke(new object(), new object[] {GetEntryArgs(entry, args)});
        }
        
        private void ShowOutputMessage()
            => OutputColorizing.colorizeForeground(ConsoleColor.Blue, () => Console.WriteLine("Output:"));

        private string[] GetEntryArgs(MethodInfo entry, string[] preferredArgs)
            => entry.GetParameters().Length > 0 ? preferredArgs : Array.Empty<string>();
    }
}
