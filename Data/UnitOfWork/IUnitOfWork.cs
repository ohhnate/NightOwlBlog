using SimpleBlogMVC.Data.Repositories.Interfaces;

namespace SimpleBlogMVC.Data
{
    public interface IUnitOfWork
    {
        IBlogRepository BlogRepository { get; }
        Task CompleteAsync();
    }
}
