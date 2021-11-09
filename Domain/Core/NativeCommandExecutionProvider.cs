using System;
using System.Collections.Generic;
using System.Linq;
using Domain.Core.Cache;
using Domain.Core.CodeBuffers;
using Domain.Core.CodeBuffers.CodePointers;
using Domain.Core.CodeCompilers;
using Domain.Core.Commands;
using Domain.Core.Exceptions;
using Libraries.Core;
using TextCopy;

namespace Domain.Core
{
    public delegate void ExecuteCommandDelegate(NativeCommand command);

    public class NativeCommandExecutionProvider
    {
        public static ICodeBuffer? SourceCodeBuffer { get; set; }
        private static readonly ICache<byte[]> Cache = new CompiledCodeCache();
        private static readonly ICodeCompiler Compiler = new SourceCodeCompiler(SourceCodeBuffer!);
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

        public static void AddCommand(string command, ExecuteCommandDelegate function)
            => CommandsMap.Add(command, function);

        private static readonly Dictionary<string, ExecuteCommandDelegate> CommandsMap =
            new(new KeyValuePair<string, ExecuteCommandDelegate>[]
                {
                    new(":quit", ExecuteQuitCommand),
                    new(":view", ExecuteViewCommand),
                    new(":write", ExecuteWriteCommand),
                    new(":writeAfter", ExecuteWriteAfterCommand),
                    new(":appendLine", ExecuteAppendLineCommand),
                    new(":enter", ExecuteEnterCommand),
                    new(":backspace", ExecuteBackspaceCommand),
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
                    new(":info", ExecuteInfoCommand),
                    new(":tab", ExecuteTabCommand),
                    new(":undo", ExecuteUndoCommand),
                    new(":redo", ExecuteRedoCommand),
                    new(":doNothing", ExecuteDoNothingCommand)
                }.ToList()
            );

        public static readonly List<string> SupportedCommands = CommandsMap.Keys.ToList();

        #region Commands
        private static void ExecuteQuitCommand(NativeCommand command)
        {
            ExecuteClearCommand(command);
            throw new InterruptedExecutionException("You have came out of the doocutor! Good bye!");
        }

        private static void ExecuteViewCommand(NativeCommand command)
        {
            Console.Clear();
            Console.WriteLine(SourceCodeBuffer!.CodeWithLineNumbers + "\n");
        }

        private static void ExecuteWriteAfterCommand(NativeCommand command)
            => SourceCodeBuffer!.WriteAfter(command.GetFirstArgumentAsAnInteger(), command.GetArgumentsSinceSecondAsALine());

        private static void ExecuteCompileCommand(NativeCommand command)
            => Cache.Cache(SourceCodeBuffer!.Code, Compiler.Compile());

        private static void ExecuteRunCommand(NativeCommand command)
        {
            Executor.Execute(Cache.HasKey(SourceCodeBuffer!.Code)
                ? Cache.GetValue(SourceCodeBuffer.Code)
                : Compiler.Compile(), command.GetArguments());
        }

        private static void ExecuteClearCommand(NativeCommand command)
            => Console.Clear();

        private static void ExecuteUsingCommand(NativeCommand command)
        {
            if (SourceCodeBuffer!.Code.Trim().StartsWith("namespace"))
                SourceCodeBuffer.WriteBefore(1, "");

            SourceCodeBuffer.WriteBefore(1, $"using {command.GetArguments()[0]};");
        }

        private static void ExecuteCopyCommand(NativeCommand command)
            => CopyText(SourceCodeBuffer!.GetLineAt(command.GetFirstArgumentAsAnInteger()));

        private static void ExecuteCopyAllCommand(NativeCommand command)
            => CopyText(SourceCodeBuffer!.Code);

        private static void ExecuteCopyBlockCommand(NativeCommand command)
            => CopyText(string.Join("\n", SourceCodeBuffer!.GetCodeBlock(
                new CodeBlockPointer(int.Parse(command.GetArguments()[0]), int.Parse(command.GetArguments()[1])))));

        private static void ExecuteRemoveCommand(NativeCommand command)
        {
            CopyText(SourceCodeBuffer!.GetLineAt(command.GetFirstArgumentAsAnInteger()));
            SourceCodeBuffer.RemoveLineAt(command.GetFirstArgumentAsAnInteger());
        }

        private static void ExecuteRemoveBlockCommand(NativeCommand command)
        {
            var arguments = command.GetArguments();
            var pointer = new CodeBlockPointer(int.Parse(arguments[0]), int.Parse(arguments[1]));
            SourceCodeBuffer!.RemoveCodeBlock(pointer);
        }

        private static void ExecuteReplaceCommand(NativeCommand command)
        {
            var arguments = command.GetArguments();
            SourceCodeBuffer!.ReplaceLineAt(int.Parse(arguments[0]), command.GetArgumentsSinceSecondAsALine());
        }

        private static void ExecuteWriteCommand(NativeCommand command)
            => SourceCodeBuffer!.Write(command.GetArgumentsAsALine());

        private static void ExecuteSetCommand(NativeCommand command)
            => SourceCodeBuffer!.SetCursorPositionFromTopAt(int.Parse(command.GetArguments()[0]));

        private static void ExecuteShowPosCommand(NativeCommand command)
        {
            Console.Write("Current cursor position: ");
            OutputColorizing.colorizeForeground(ConsoleColor.Cyan,
                () => Console.Write(SourceCodeBuffer!.CursorPositionFromTop + "\n"));
        }

        private static void ExecuteAddRefCommand(NativeCommand command)
            => Compiler.AddReference(FileSystem.getGlobalPath(command.GetArguments()[0]));

        private static void ExecuteSaveCodeCommand(NativeCommand command)
            => SourceCodeSaving.saveCode(command.GetArgumentsAsALine(), SourceCodeBuffer!.Lines);

        private static void ExecuteSaveAsmCommand(NativeCommand command)
        {
            if (!Cache.HasKey(SourceCodeBuffer!.Code))
                Cache.Cache(SourceCodeBuffer.Code, Compiler.Compile());

            AssemblySaving.saveAssembly(command.GetArgumentsAsALine(), Cache.GetValue(SourceCodeBuffer.Code));
        }

        private static void ExecuteHelpCommand(NativeCommand command)
            => HelpList.Show();

        private static void ExecuteInfoCommand(NativeCommand command)
            => Console.WriteLine(Info.Description);

        private static void ExecuteAppendLineCommand(NativeCommand command)
            => SourceCodeBuffer!.AppendLine(string.Join(" ", command.Content.Split(" ")[1..]));

        private static void ExecuteEnterCommand(NativeCommand command)
            => SourceCodeBuffer!.Enter();

        private static void ExecuteBackspaceCommand(NativeCommand command)
            => SourceCodeBuffer!.Backspace();

        private static void ExecuteTabCommand(NativeCommand command)
            => SourceCodeBuffer!.AppendLine("    ");

        private static void ExecuteDoNothingCommand(NativeCommand command) { }

        private static void ExecuteUndoCommand(NativeCommand command)
            => SourceCodeBuffer!.Undo();

        private static void ExecuteRedoCommand(NativeCommand command)
            => SourceCodeBuffer!.Redo();

        #endregion

        private static void CopyText(string text)
            => new Clipboard().SetText(text);
    }
}
