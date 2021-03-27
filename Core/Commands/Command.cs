
namespace Doocutor.Core.Commands
{
    interface Command
    {
        CommandType Type { get; }
        string Content { get; }
    }
}
