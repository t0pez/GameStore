using System;
using System.Collections.Generic;
using System.Text;

namespace GameStore.Core.Exceptions
{
    [Serializable]
    public class ItemNotFoundException : Exception
    {
        public ItemNotFoundException()
        {
        }

        public ItemNotFoundException(string message) : base(message)
        {
        }
        
        public ItemNotFoundException(string message, Exception inner) : base(message, inner)
        {
        }
    }
}
