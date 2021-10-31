using System;
using CommandLine;
using Domain.Core;
using Domain.Core.Exceptions;
using Domain.Options;
using DynamicEditor;
using Libraries.Core;
using NLog;

namespace Doocutor
{
    public static class Program
    {
        private static readonly Logger Logger = LogManager.GetLogger("Doocutor.Program");

        public static void Main(string[] args)
        {
            try
            {
                Start();

                new DynamicEditorSetup().Run(ParseCommandLineArguments(args));
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
