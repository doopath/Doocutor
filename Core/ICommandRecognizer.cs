using Doocutor.Core.Commands;

namespace Doocutor.Core
{
    interface ICommandRecognizer
    {
        Command Recognize(string command);
    }
}
