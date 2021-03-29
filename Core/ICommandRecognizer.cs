using Doocutor.Core.Commands;

namespace Doocutor.Core
{
    internal interface ICommandRecognizer
    {
        ICommand Recognize(string command);
    }
}
