using Domain.Options;

namespace Domain.Core
{
    public interface IEditorSetup
    {
        void Run(ProgramOptions options);
    }
}
