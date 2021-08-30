using Domain.Core.Commands;

namespace Domain.Core
{
    public interface ICommandRecognizer
    {
        ICommand Recognize(string command);
    }
}
