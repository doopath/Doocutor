using System;
using System.IO;
using CommandLine;
using Domain.Core;
using Domain.Core.Exceptions;
using Doocutor.Options;
using DynamicEditor;
using Libraries.Core;
using NLog;
using StaticEditor;

namespace Doocutor
{
    public static class Program
    {
        private static readonly Logger Logger = LogManager.GetLogger("Doocutor.Program");

        public static void Main(string[] args)
        {
            try
            {
                var ops = ParseCommandLineArguments(args);
                var editor = SelectEditor(ops.EditorMode);

                Start();

                editor.Run(args);
            }
            catch (InterruptedExecutionException error)
            {
                ErrorHandling.handleInterruptedExecutionException(error, End);
            }
            catch (Exception error)
            {
                ErrorHandling.showError(error);
            }
            finally
            {
                End();
            }
        }

        private static IEditorSetup SelectEditor(string? mode) => mode?.ToLower() switch
        {
            "dynamic" => new DynamicEditorSetup(),
            "static" => new StaticEditorSetup(),
            null => new StaticEditorSetup(),
            _ => throw new ArgumentException($"Cannot run editor in mode={mode}")
        };


        private static void Start()
            => OutputColorizing.colorizeForeground(ConsoleColor.Cyan, () =>
            {
                Logger.Debug("Start of the program");
                Info.ShowDoocutorInfo();
            });

        private static void End()
            => OutputColorizing.colorizeForeground(ConsoleColor.Cyan,
                () =>
                {
                    Logger.Debug("End of the program\n\n");
                    OutputColorizing.colorizeForeground(ConsoleColor.Cyan, () => Console.WriteLine("\nGood bye!\n"));
                });

        private static ProgramOptions ParseCommandLineArguments(string[] args)
        {
            var result = new ProgramOptions();

            Parser
                .Default
                .ParseArguments<ProgramOptions>(args)
                .WithParsed(ops => result = ops);

            return result;
        }
    }
}
