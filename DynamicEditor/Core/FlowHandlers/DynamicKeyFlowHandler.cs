using System;
using Domain.Core.FlowHandlers;
using Domain.Core.Iterators;
using Domain.Core.CommandHandlers;
using System.Collections.Generic;
using Domain.Core.Exceptions;

namespace DynamicEditor.Core.FlowHandlers
{
    internal sealed class DynamicKeyFlowHandler : IInputFlowHandler
    {
        private readonly IInputFlowIterator _inputFlowIterator;
        private readonly ICommandHandler _commandHandler;
        private readonly KeyCombinationTranslating _keyCombinationTranslating;
        private readonly CuiRender _cuiRender;

        public bool IsClosed { get; private set; }

        public DynamicKeyFlowHandler(
            IInputFlowIterator iterator,
            ICommandHandler commandHandler,
            Dictionary<string, string> keyCombinationsMap,
            CuiRender cuiRender)
        {
            _inputFlowIterator = iterator;
            _commandHandler = commandHandler;
            _keyCombinationTranslating = new KeyCombinationTranslating(keyCombinationsMap);
            _cuiRender = cuiRender;
            IsClosed = false;
        }

        public void StartHandling()
        {
            while (_inputFlowIterator.HasNext())
            {
                var input = _inputFlowIterator.Next();
                var command = _keyCombinationTranslating.TryGetCommandFor(input) ?? input;

                if (MovementKeysMap.Map.ContainsKey(input))
                {
                    try
                    {
                        MovementKeysMap.Map[input](_cuiRender);
                    }
                    catch (OutOfCodeBufferSizeException){}
                }
                else
                {
                    _commandHandler.Handle(command);
                    _cuiRender.Render();
                }
            }


            IsClosed = true;
        }
    }
}
