using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Doocutor.Core.Exceptions
{
    class OutOfCodeBufferSizeException : Exception
    {
        public OutOfCodeBufferSizeException(string message) : base(message) {}
    }
}
