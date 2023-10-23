using Currency.Models;
using System.Runtime.Serialization;
using System.Text;

namespace Currency.UserClasses
{
    [Serializable]
    internal class CurrencyException : Exception
    {
        public CurrencyException()
        {
        }

        public CurrencyException(string? message) : base(message)
        {
        }

        public CurrencyException(string? message, Exception? innerException) : base(message, innerException)
        {
        }

        protected CurrencyException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine($"Exception Message: {this.Message}");
            sb.AppendLine($"Object: {this.Source}");
            sb.AppendLine($"StackTrace: {this.StackTrace}"); 
            return sb.ToString();            
        }
    }
}