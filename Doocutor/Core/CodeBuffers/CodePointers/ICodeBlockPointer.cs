using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Doocutor.Core.CodeBuffers.CodePointers
{
    interface ICodeBlockPointer
    {
        int StartLineNumber { get; }
        int EndLineNumber { get; }
    }
}
