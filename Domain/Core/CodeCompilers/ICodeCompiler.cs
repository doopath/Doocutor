using System.Reflection;

namespace Domain.Core.CodeCompilers
{
    public interface ICodeCompiler
    {
        byte[] Compile();
        void AddReference(string newReference);
    }
}
