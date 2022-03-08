using System;

namespace GameStore.Core.Exceptions
{
    [Serializable]
    public class ItemAlreadyExistsException : Exception
    {
        public ItemAlreadyExistsException()
        {
        }

        public ItemAlreadyExistsException(string message) : base(message)
        {
        }

        public ItemAlreadyExistsException(string message, Exception inner) : base(message, inner)
        {
        }
    }
}
