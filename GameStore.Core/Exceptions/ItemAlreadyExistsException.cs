using System;
using System.Collections.Generic;
using System.Text;

namespace GameStore.Core.Exceptions
{
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
