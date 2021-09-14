using Domain.Core.Commands;

namespace Domain.Core.CommandRecognizers
{
    public interface ICommandRecognizer
    {
        ICommand Recognize(string command);
    }
}
