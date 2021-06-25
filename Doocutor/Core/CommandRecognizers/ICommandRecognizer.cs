using Doocutor.Core.Commands;

namespace Doocutor.Core
{
    public interface ICommandRecognizer
    {
        ICommand Recognize(string command);
    }
}
