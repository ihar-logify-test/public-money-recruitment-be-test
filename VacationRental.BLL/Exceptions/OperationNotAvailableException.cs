using System;

namespace VacationRental.BLL.Exceptions
{
    public class OperationNotAvailableException : Exception
    {
        public OperationNotAvailableException(string message) : base(message)
        {
        }
    }
}