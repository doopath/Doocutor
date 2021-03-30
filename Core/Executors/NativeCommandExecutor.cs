using System;
using System.Collections.Generic;

using Doocutor.Core;
using Doocutor.Core.Commands;

namespace Doocutor.Core.Executors
{
    internal class NativeCommandExecutor : ICommandExecutor<NativeCommand>
    {
        public void Execute(NativeCommand command)
        {
            NativeCommander.GetExecutingFunction(command).Invoke();
        }
    }
}
