using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Domain.Core.CommandHandlers;
using Domain.Core.Exceptions;
using Domain.Core.FlowHandlers;
using Domain.Core.Iterators;

namespace DynamicEditor.Core.FlowHandlers
{
    internal sealed class DynamicKeyFlowHandler : IInputFlowHandler
    {
        private readonly IInputFlowIterator _inputFlowIterator;
        private readonly ICommandHandler _commandHandler;
        private readonly KeyCombinationTranslating _keyCombinationTranslating;

        public bool IsClosed { get; private set; }

        public DynamicKeyFlowHandler(
            IInputFlowIterator iterator,
            ICommandHandler commandHandler,
            Dictionary<string, string> keyCombinationsMap)
        {
            _inputFlowIterator = iterator;
            _commandHandler = commandHandler;
            _keyCombinationTranslating = new KeyCombinationTranslating(keyCombinationsMap);
            IsClosed = false;
        }

        public void StartHandling()
        {
            while (_inputFlowIterator.HasNext())
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
                    catch (OutOfCodeBufferSizeException) { }
                }
                else
                {
                    _commandHandler.Handle(command);
                    CuiRender.Render();
                }
            }

            IsClosed = true;
        }

        private bool IsMatchedWithAPattern(string input)
            => KeyCombinationsMap.Map.Keys
                .Any(key => Regex.IsMatch(input, key, RegexOptions.IgnoreCase));

        private string GetCommand(string input)
        {
            foreach (var key in KeyCombinationsMap.Map.Keys)
            {
                if (Regex.Match(input, key).Success)
                    return KeyCombinationsMap.Map[key];
            }

            return string.Empty;
        }

        private bool IsTheAppendLineCommand(string command)
            => command == ":appendLine ";
    }
}
