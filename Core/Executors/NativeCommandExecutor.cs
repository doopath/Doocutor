using Doocutor.Core.Commands;
using Doocutor.Core.CodeBuffers;
using System;

namespace Doocutor.Core.Executors
{
    internal class NativeCommandExecutor : ICommandExecutor<NativeCommand>
    {
        public void Execute(NativeCommand command)
        {
            var code = new SourceCodeBuffer();
            Console.WriteLine(code.Code);

            code.Write("System.Console.WriteLine(\"Hello World!\");");
            Console.WriteLine(code.Code);

            code.Write("System.Console.WriteLine(\"This is my first doocutor writing!\");");
            Console.WriteLine(code.Code);

            NativeCommander.GetExecutingFunction(command)();
        }
    }
}
