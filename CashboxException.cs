using System;

namespace CashierWebApp
{
    public class CashboxException : Exception
    {
        public CashboxException(string message) : base(message)
        { }
    }
}
