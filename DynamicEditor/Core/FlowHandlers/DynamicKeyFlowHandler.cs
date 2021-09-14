using Domain.Core.FlowHandlers;
using Domain.Core.Iterators;
using Domain.Core.CommandHandlers;
using System.Collections.Generic;

namespace DynamicEditor.Core.FlowHandlers
{
    internal sealed class DynamicKeyFlowHandler : IInputFlowHandler
    {
        private readonly IInputFlowIterator _inputFlowIterator;
        private readonly ICommandHandler _commandHandler;
        private readonly KeyCombinationTranslating _keyCombinationTranslating;
        private readonly CUIRender _CUIRender;

        public bool IsClosed { get; private set; } = false;

        public DynamicKeyFlowHandler(
            IInputFlowIterator iterator,
            ICommandHandler commandHandler,
            Dictionary<string, string> keyCombinationsMap,
            CUIRender cuiRender)
        {
            _inputFlowIterator = iterator;
            _commandHandler = commandHandler;
            _keyCombinationTranslating = new KeyCombinationTranslating(keyCombinationsMap);
            _CUIRender = cuiRender;
        }

        public void StartHandling()
        {
            while (_inputFlowIterator.HasNext())
            {
                var input = _inputFlowIterator.Next();
                var command = _keyCombinationTranslating.TryGetCommandFor(input) ?? input;

                _commandHandler.Handle(command);

                _CUIRender.Render();
            }


            IsClosed = true;
        }
    }
}
