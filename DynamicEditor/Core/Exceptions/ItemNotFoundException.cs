using System;

namespace DynamicEditor.Core.Exceptions
{
    internal class ItemNotFoundException : Exception
    {
        public ItemNotFoundException(string message) : base(message) { }
    }
}
