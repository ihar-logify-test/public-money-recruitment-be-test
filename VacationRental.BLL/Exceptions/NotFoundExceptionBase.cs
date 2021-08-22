using System;
namespace VacationRental.BLL.Exceptions
{
    public class NotFoundExceptionBase : Exception
    {
        public NotFoundExceptionBase(string message) : base(message)
        {
        }
    }
}