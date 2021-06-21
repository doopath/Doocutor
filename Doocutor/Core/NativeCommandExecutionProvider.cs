using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using TextCopy;
using Doocutor.Core.Caches;
using Doocutor.Core.CodeBuffers;
using Doocutor.Core.CodeBuffers.CodePointers;
using Doocutor.Core.CodeCompilers;
using Doocutor.Core.Commands;
using Doocutor.Core.Exceptions;
using Libraries.Core;

namespace Doocutor.Core
{
    public delegate void ExecuteCommandDelegate(NativeCommand command);

    public class NativeCommandExecutionProvider
    {
        private static readonly ICodeBuffer SourceCode = new SourceCodeBuffer();
        private static readonly ICache<byte[]> Cache = new CompiledCodeCache();
        private static readonly ICodeCompiler Compiler = new SourceCodeCompiler(SourceCode);
        private static readonly CompiledCodeExecutor Executor = new();

        /// <summary>
        /// Get a function (as an action) for a given command to execute that.
        /// </summary>
        /// <exception cref="UnsupportedCommandException">
        /// Throws that if a given command is not included in the list of supported commands.
        /// </exception>
        public static Action GetExecutingFunction(NativeCommand command)
        {
            var content = command.Content.Split(" ")[0];

            if (SupportedCommands.Contains(content))
                return () => CommandsMap[content].Invoke(command);

            throw new UnsupportedCommandException($"Given command ({command.Content}) is not supported!");
        }

        /// <summary>
        /// Using for unit-testing.
        /// </summary>
        protected static void AddExecutingCommand(string command, ExecuteCommandDelegate function)
            => CommandsMap.Add(command, function);

        private static readonly Dictionary<string, ExecuteCommandDelegate> CommandsMap =
            new(new KeyValuePair<string, ExecuteCommandDelegate>[]
                {
                    new(":quit", ExecuteQuitCommand),
                    new(":view", ExecuteViewCommand),
                    new(":write", ExecuteWriteCommand),
                    new(":writeAfter", ExecuteWriteAfterCommand),
                    new(":compile", ExecuteCompileCommand),
                    new(":run", ExecuteRunCommand),
                    new(":using", ExecuteUsingCommand),
                    new(":copy", ExecuteCopyCommand),
                    new(":copyAll", ExecuteCopyAllCommand),
                    new(":copyBlock", ExecuteCopyBlockCommand),
                    new(":clear", ExecuteClearCommand),
                    new(":remove", ExecuteRemoveCommand),
                    new(":removeBlock", ExecuteRemoveBlockCommand),
                    new(":replace", ExecuteReplaceCommand),
                    new(":set", ExecuteSetCommand),
                    new(":showPos", ExecuteShowPosCommand),
                    new(":addRef", ExecuteAddRefCommand),
                    new(":saveCode", ExecuteSaveCodeCommand),
                    new(":saveAsm", ExecuteSaveAsmCommand),
                    new(":help", ExecuteHelpCommand),
                    new(":info", ExecuteInfoCommand)
                }.ToList()
            );

        public static readonly List<string> SupportedCommands = CommandsMap.Keys.ToList();

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
            => SourceCode.WriteAfter(command.GetFirstArgumentAsAnInteger(), command.GetArgumentsSinceSecondAsALine());

        private static void ExecuteCompileCommand(NativeCommand command)
            => Cache.Cache(SourceCode.Code, Compiler.Compile());

        private static void ExecuteRunCommand(NativeCommand command)
        {
            Executor.Execute(Cache.HasKey(SourceCode.Code)
                ? Cache.GetValue(SourceCode.Code)
                : Compiler.Compile(), command.GetArguments());
        }

        private static void ExecuteClearCommand(NativeCommand command) => Console.Clear();

        private static void ExecuteUsingCommand(NativeCommand command)
        {
            if (SourceCode.Code.Trim().StartsWith("namespace"))
                SourceCode.WriteBefore(1, "");

            SourceCode.WriteBefore(1, $"using {command.GetArguments()[0]};");
        }

        private static void ExecuteCopyCommand(NativeCommand command)
            => CopyText(SourceCode.GetLineAt(command.GetFirstArgumentAsAnInteger()));

        private static void ExecuteCopyAllCommand(NativeCommand command)
            => CopyText(SourceCode.Code);

        private static void ExecuteCopyBlockCommand(NativeCommand command)
            => CopyText(string.Join("\n", SourceCode.GetCodeBlock(
                new CodeBlockPointer(int.Parse(command.GetArguments()[0]), int.Parse(command.GetArguments()[1])))));

        private static void CopyText(string text)
            => new Clipboard().SetText(text);

        private static void ExecuteRemoveCommand(NativeCommand command)
        {
            CopyText(SourceCode.GetLineAt(command.GetFirstArgumentAsAnInteger()));
            SourceCode.RemoveLineAt(command.GetFirstArgumentAsAnInteger());
        }

        private static void ExecuteRemoveBlockCommand(NativeCommand command)
        {
            var arguments = command.GetArguments();
            var pointer = new CodeBlockPointer(int.Parse(arguments[0]), int.Parse(arguments[1]));
            SourceCode.RemoveCodeBlock(pointer);
        }

        private static void ExecuteReplaceCommand(NativeCommand command)
        {
            var arguments = command.GetArguments();
            SourceCode.ReplaceLineAt(int.Parse(arguments[0]), command.GetArgumentsSinceSecondAsALine());
        }

        private static void ExecuteWriteCommand(NativeCommand command)
            => SourceCode.Write(command.GetArgumentsAsALine());

        private static void ExecuteSetCommand(NativeCommand command)
            => SourceCode.SetPointerPositionAt(int.Parse(command.GetArguments()[0]));

        private static void ExecuteShowPosCommand(NativeCommand command)
        {
            Console.Write("Current cursor position: ");
            OutputColorizer.colorizeForeground(ConsoleColor.Cyan,
                () => Console.Write(SourceCode.CurrentPointerPosition + "\n"));
        }

        private static void ExecuteAddRefCommand(NativeCommand command)
            => Compiler.AddReference(command.GetArguments()[0]);

        private static void ExecuteSaveCodeCommand(NativeCommand command)
            => SourceCodeSaver.saveCode(command.GetArgumentsAsALine(), SourceCode.Lines);

        private static void ExecuteSaveAsmCommand(NativeCommand command)
        {
            if (!Cache.HasKey(SourceCode.Code))
                Cache.Cache(SourceCode.Code, Compiler.Compile());

            AssemblySaver.saveAssembly(command.GetArgumentsAsALine(), Cache.GetValue(SourceCode.Code));
        }

        private static void ExecuteHelpCommand(NativeCommand command)
            => Console.WriteLine(Info.HelpList);

        private static void ExecuteInfoCommand(NativeCommand command)
            => Console.WriteLine(Info.Description);
    }
}
