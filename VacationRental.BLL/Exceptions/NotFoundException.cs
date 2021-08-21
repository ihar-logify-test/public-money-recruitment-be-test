using System;

namespace VacationRental.BLL.Exceptions
{
    public class NotFoundException<TId> : NotFoundExceptionBase
    {
        private readonly TId _resourceId;
        private readonly string _resourceName;

        public NotFoundException(TId resourceId, string resourceName) : base($"'{resourceName}' with id '{resourceId}' not found")
        {
            _resourceId = resourceId;
            _resourceName = resourceName;
        }
    }
}