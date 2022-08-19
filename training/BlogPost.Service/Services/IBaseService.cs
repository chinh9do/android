namespace BlogPost.Service.Services;

public interface IBaseService<T>
{
    Task<IEnumerable<T>> GetAsync();
    Task<T> GetByIdAsync(string id);
    //Task AddAsync(T item);
    Task UpdateAsync(T item);
    Task DeleteAsync(string id);
}
