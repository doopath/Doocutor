using System.Collections.Generic;
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
        private readonly KeyToSymbolTranslating _keyToSymbolTranslating;
        private readonly CuiRender _cuiRender;

        public bool IsClosed { get; private set; }

        public DynamicKeyFlowHandler(
            IInputFlowIterator iterator,
            ICommandHandler commandHandler,
            Dictionary<string, string> keyCombinationsMap,
            Dictionary<string, string> keyMap,
            CuiRender cuiRender)
        {
            _inputFlowIterator = iterator;
            _commandHandler = commandHandler;
            _keyCombinationTranslating = new KeyCombinationTranslating(keyCombinationsMap);
            _keyToSymbolTranslating = new KeyToSymbolTranslating(keyMap);
            _cuiRender = cuiRender;
            IsClosed = false;
        }

        public void StartHandling()
        {
            while (_inputFlowIterator.HasNext())
            {
                var input = _inputFlowIterator.Next();
                var command = _keyCombinationTranslating.GetCommandFor(input);
                command = command is "" ? input : command;
                command = IsMatchedWithAPattern(input) ? GetCommand(input) : command;

                if (IsTheAppendLineCommand(command))
                    command += ConvertToASymbol(input);

                if (MovementKeysMap.Map.ContainsKey(input))
                {
                    try
                    {
                        MovementKeysMap.Map[input](_cuiRender);
                    }
                    catch (OutOfCodeBufferSizeException) { }
                }
                else
                {
                    _commandHandler.Handle(command);
                    _cuiRender.Render();
                }
            }


            IsClosed = true;
        }

        private bool IsMatchedWithAPattern(string input)
        {
            foreach (var key in KeyCombinationsMap.Map.Keys)
            {
                if (Regex.IsMatch(input, key, RegexOptions.IgnoreCase))
                    return true;
            }

            return false;
        }

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

        private string ConvertToASymbol(string input)
        {
            var symbol = _keyToSymbolTranslating.GetSymbolFor(input);

            return symbol is "" ? input : symbol;
        }
    }
}
