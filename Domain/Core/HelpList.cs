using System;
using System.Linq;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Spectre.Console;

namespace Domain.Core
{
    public static class HelpList
    {
        public static string CommandsDescriptions => _helpList;

        private const string _helpList =
            ":quit -|- Exit the program.\n" +
            ":view -|- View code.\n" +
            ":write <new line> -|- Write a new line after current pointer position (see :showPos)." +
                " Also you can add a new line without a command. Just type it and press enter.\n" +
            ":writeAfter <line number> <new line> -|- Write a new line after <line number>.\n" +
            ":writeBefore <line number> <new line> -|- Write a new line before <line number>.\n" +
            ":compile -|- Compile current code.\n" +
            ":run -|- Run compiled code. You should not compile code before running. It will be compiled automatically.\n" +
            ":using <namespace> -|- Add a namespace to using list (for example: System).\n" +
            ":copy <line number> -|- Copy the <line number> content.\n" +
            ":copyAll -|- Copy entire content.\n" +
            ":copyBlock <since> <to> -|- Copy a block of code since <since> to <to> (line number).\n" +
            ":clear -|- Clear the console.\n" +
            ":remove <line number> -|- Copy and remove line at <line number>.\n" +
            ":removeBlock <since> <to> -|- Remove a block of code since <since> to <to> (line number).\n" +
            ":replace <line number> <new content> -|- Replace a line at <line number> with <new content>.\n" +
            ":addRef <path to asm> -|- Add an assembly reference. You can add a reference to a library and use it in your code.\n" +
            ":saveCode <path> -|- Save current code as a file at <path>.\n" +
            ":saveAsm <path> -|- Save compiled code as an assembly at <path>. Should have .dll extension. \n" +
            ":set <line number> -|- Set current cursor position.\n" +
            ":showPos -|- Show current position of the cursor.\n";

        public static void Show()
        {
            var table = new Table();
            table.AddColumn("[mediumpurple bold]Command[/]");
            table.AddColumn("[mediumpurple bold]Description[/]");

            foreach (var pair in ConvertCommands())
                table.AddRow(pair.Item1, pair.Item2);

            AnsiConsole.Render(table);
            AnsiConsole.WriteLine();
        }

        private static List<(string, string)> ConvertCommands()
        {
            var splitCommands = _helpList.Split("\n")[..^1];
            var result = Array.Empty<(string, string)>().ToList();

            foreach (var command in splitCommands)
            {
                var splitCommand = command.Split(" -|- ");
                result.Add(ColorizeCommandForTheTable(splitCommand));
            }

            return result;
        }

        private static string ColorizeArguments(string command)
        {
            var matches = new Regex(@"<[a-zA-Z\s]+>").Matches(command.Trim());

            if (matches.Count > 0)
            {
                foreach (var match in matches)
                    command = ColorizeMatchAsAnArgumentIn(command, match);
            }

            return command;
        }

        private static (string, string) ColorizeCommandForTheTable(string[] commandDescriptionPair)
            => ($"[mediumpurple bold]{ColorizeArguments(commandDescriptionPair[0])}[/]", ColorizeArguments(commandDescriptionPair[1]));

        private static string ColorizeMatchAsAnArgumentIn(string command, object match)
            => command.Replace(match.ToString(), $"[royalblue1 italic]{match}[/]");
    }
}