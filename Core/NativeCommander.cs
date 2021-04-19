using System;
using System.Collections.Generic;
using System.Linq;
using TextCopy;
using Doocutor.Core.Caches;
using Doocutor.Core.CodeBuffers;
using Doocutor.Core.CodeCompilers;
using Doocutor.Core.Commands;
using Doocutor.Core.Exceptions;

namespace Doocutor.Core
{
    internal delegate void ExecuteCommandDelegate(NativeCommand command);

    internal static class NativeCommander
    {
        private static readonly ICodeBuffer SourceCode = new SourceCodeBuffer();
        private static readonly ICache<byte[]> Cache = new CompiledCodeCache();
        private static readonly ICodeCompiler Compiler = new SourceCodeCompiler(SourceCode);
        private static readonly CompiledCodeExecutor Executor = new();

        public static readonly List<string> SupportedCommands = new(new[]
        {
            ":quit",
            ":view",
            ":compile",
            ":run",
            ":writeAfter",
            ":writeBefore",
            ":using",
            ":copy",
            ":remove",
            ":removeBlock",
            ":replace"
        }.ToList());

        private static readonly Dictionary<string, ExecuteCommandDelegate> CommandsMap =
            new(new KeyValuePair<string, ExecuteCommandDelegate>[]
                {
                    new(":quit", ExecuteQuitCommand),
                    new(":view", ExecuteViewCommand),
                    new(":writeAfter", ExecuteWriteAfterCommand),
                    new(":compile", ExecuteCompileCommand),
                    new(":run", ExecuteRunCommand),
                    new(":using", ExecuteUsingCommand),
                    new(":copy", ExecuteCopyCommand),
                    new(":remove", ExecuteRemoveCommand)
                }.ToList()
            );

        public static Action GetExecutingFunction(NativeCommand command)
        {
            var function = CommandsMap[command.Content.Split(" ")[0]] 
                ?? throw new UnsupportedCommandException($"Given command {command.Content} is not supported!");

            return () => function.Invoke(command);
        }

        private static void ExecuteQuitCommand(NativeCommand command)
        {
            throw new InterruptedExecutionException("You have came out of the doocutor! Good bye!");
        }

        private static void ExecuteViewCommand(NativeCommand command)
        {
            Console.Clear();
            Console.WriteLine(SourceCode.CodeWithLineNumbers);
        }

        private static void ExecuteWriteAfterCommand(NativeCommand command)
        {
            try
            {
                SourceCode.WriteAfter(command.GetTargetLineNumber(), command.GetTargetLine());
            }
            catch (OutOfCodeBufferSizeException error)
            {
                ErrorHandler.ShowError(error);
            }
            catch (CommandExecutingException error)
            {
                ErrorHandler.ShowError(error);
            }
        }

        private static void ExecuteCompileCommand(NativeCommand command)
        {
            Cache.Cache(SourceCode.Code, Compiler.Compile());
        }

        private static void ExecuteRunCommand(NativeCommand command)
        {
            Executor.Execute(Cache.HasKey(SourceCode.Code)
                ? Cache.GetValue(SourceCode.Code)
                : Compiler.Compile(), command.GetArguments());
        }

        private static void ExecuteUsingCommand(NativeCommand command)
        {
            if (SourceCode.Code.Trim().StartsWith("namespace"))
                SourceCode.WriteBefore(1, "");
            
            SourceCode.WriteBefore(1, $"using {command.GetArguments()[0]};");
        }

        private static void ExecuteCopyCommand(NativeCommand command)
            => new Clipboard().SetText(SourceCode.Code);

        private static void ExecuteRemoveCommand(NativeCommand command)
        {
            try
            {
                SourceCode.RemoveLine(int.Parse(command.GetArguments()[0]));
            }
            catch (Exception error)
            {
                ErrorHandler.ShowError(error);
            }
            
        }
    }
}