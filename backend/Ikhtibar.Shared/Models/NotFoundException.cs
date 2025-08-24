using System;

namespace Ikhtibar.Shared.Models
{
    [Serializable]
    public class NotFoundException : Exception
    {
        public NotFoundException() : base("Resource not found") { }
        public NotFoundException(string message) : base(message) { }
        public NotFoundException(string message, Exception inner) : base(message, inner) { }
    }
}
