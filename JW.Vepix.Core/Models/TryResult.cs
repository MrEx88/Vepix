using System;

namespace JW.Vepix.Core.Models
{
    public class TryResult
    {
        public bool? Success { get; private set; }
        public string Message { get; private set; }

        //public TryResult()
        //    : this(string.Empty)
        //{

        //}

        //public TryResult(string message)
        //{
        //    Success = message.Length == 0;
        //    Message = message;
        //}

        public void Try(Action action)
        {
            try
            {
                action();
            }
            catch (Exception e)
            {
                Message = e.Message;
            }

            Success = Message.Length == 0;
        }
    }
}
