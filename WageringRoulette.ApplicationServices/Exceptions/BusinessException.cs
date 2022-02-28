using System;
using System.Collections.Generic;

namespace WageringRoulette.ApplicationServices.Exceptions
{
    [Serializable]
    public class BusinessException : Exception
    {
        public virtual Dictionary<string, string> Errors { get; }

        public BusinessException(string errorCode, string message, Exception inner) : base(message, inner)
        {
            this.Errors = new Dictionary<string, string>
            {
                { errorCode, message }
            };
        }

        public BusinessException(string errorCode, string message) : base(message)
        {
            this.Errors = new Dictionary<string, string>
            {
                { errorCode, message }
            };
        }

        public BusinessException(Dictionary<string, string> errors)
        {
            if (errors == null)
            {
                this.Errors = new Dictionary<string, string>();
            }

            this.Errors = errors;
        }
    }
}
