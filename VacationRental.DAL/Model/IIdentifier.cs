namespace VacationRental.DAL.Model
{
    public interface IIdentifier<TKey>
    {
        TKey Id { get; set; }
    }
}