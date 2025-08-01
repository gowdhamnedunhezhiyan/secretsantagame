using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SecretSantaGame.Exceptions
{
    public class SecretSantaException : Exception
    {
        public SecretSantaException(string message) : base(message) { }

        public SecretSantaException(string message, Exception innerException) : base(message, innerException) { }
    }
}
