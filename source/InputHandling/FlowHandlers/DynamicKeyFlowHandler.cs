﻿using CommandHandling.CommandHandlers;
using CUI;
using Domain.Core.Iterators;
using InputHandling.CommandHandlers;
using InputHandling.Iterators;
using System.Text.RegularExpressions;
using Utils.Exceptions.NotExitExceptions;

namespace InputHandling.FlowHandlers
{
    public class DynamicKeyFlowHandler : IInputFlowHandler
    {
        protected readonly IInputFlowIterator _inputFlowIterator;
        protected readonly ICommandHandler _commandHandler;
        protected readonly KeyCombinationTranslating _keyCombinationTranslating;

        public virtual bool IsClosed { get; private set; }

        public DynamicKeyFlowHandler(
            DynamicConsoleInputFlowIterator iterator,
            DynamicCommandHandler commandHandler,
            Dictionary<string, string> keyCombinationsMap)
        {
            _inputFlowIterator = iterator;
            _commandHandler = commandHandler;
            _keyCombinationTranslating = new KeyCombinationTranslating(keyCombinationsMap);
            IsClosed = false;
        }

        public virtual void StartHandling()
        {
            while (_inputFlowIterator.HasNext() && IsClosed is false)
            {
                ConsoleKeyInfo input = _inputFlowIterator.Next();
                string keyCombination = input.ToKeyCombination();
                string keySymbol = input.KeyChar.ToString();

                string command = _keyCombinationTranslating.GetCommandFor(keyCombination);
                command = command is "" ? keySymbol : command;
                command = IsMatchedWithAPattern(keyCombination) ? GetCommand(keyCombination) : command;

                if (IsTheAppendLineCommand(command))
                    command += keySymbol;

                if (MovementKeysMap.Map.ContainsKey(keyCombination))
                {
                    try
                    {
                        MovementKeysMap.Map[keyCombination]();
                    }
                    catch (OutOfTextBufferSizeException) { }
                }
                else
                {
                    _commandHandler.Handle(command);
                    CuiRender.Render();
                }
            }

            Close();
        }

        public virtual void StopHandling()
            => IsClosed = true;

        public virtual void Open()
            => IsClosed = false;

        protected virtual void Close()
            => IsClosed = true;

        protected virtual bool IsMatchedWithAPattern(string input)
            => KeyCombinationsMap.Map.Keys
                .Any(key => Regex.IsMatch(input, key, RegexOptions.IgnoreCase));

        protected virtual string GetCommand(string input)
        {
            IEnumerable<string> matchedKeys = KeyCombinationsMap
                .Map
                .Keys
                .Where(key => Regex.Match(input, key).Success);

            foreach (var key in matchedKeys)
                return KeyCombinationsMap.Map[key];

            return string.Empty;
        }

        protected virtual bool IsTheAppendLineCommand(string command)
            => command == ":appendLine ";
    }
}
