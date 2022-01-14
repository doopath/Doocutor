using System;
using System.Linq;
using CommandLine;
using Common.Options;
using Utils.Exceptions;
using CUI;
using NLog;
using Utils;

namespace Doocutor
{
    public static class Program
    {
        private static Logger Logger { get; }

        static Program()
        {
            Logger = LogManager.GetLogger("Doocutor.Program");
        }

        public static void Main(string[] args)
        {
            try
            {
                AppOptions? options = ParseCommandLineArguments(args);

                if (HasHelpOption(args)) return;

                Start();
                new App().Run(options);

            }
            catch (InterruptedExecutionException error)
            {
                ErrorHandling.HandleInterruptedExecutionException(error, End);
            }
            catch (Exception error)
            {
                ErrorHandling.ShowError(error);
            }
            finally
            {
                End();
            }
        }

        private static void Start()
            => OutputColorizing.ColorizeForeground(ConsoleColor.Cyan, () =>
            {
               Logger.Debug("Start of the program");
               Info.ShowDoocutorInfo();
            });

        private static void End()
            => OutputColorizing.ColorizeForeground(ConsoleColor.Cyan,
                () =>
                {
                    string endMessage = "End of the program\n";
                    Logger.Debug($"{endMessage}\n");
                    OutputColorizing.ColorizeForeground(ConsoleColor.Cyan,
                       () => Console.WriteLine($"\n{endMessage}"));
                });

        private static AppOptions ParseCommandLineArguments(string[] args)
        {
            var result = new AppOptions();

            Parser
                .Default
                .ParseArguments<AppOptions>(args)
                .WithParsed(ops => result = ops);

            return result;
        }

        private static bool HasHelpOption(string[] args)
            => args.Contains("--help");
    }
}
