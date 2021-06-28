using System.Reflection;

namespace Doocutor.Core.CodeCompilers
{
    internal interface ICodeCompiler
    {
        byte[] Compile();
        void AddReference(string newReference);
    }
}
