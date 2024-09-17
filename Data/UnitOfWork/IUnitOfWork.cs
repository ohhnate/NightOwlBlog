using SimpleBlogMVC.Data.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace SimpleBlogMVC.Data
{
    public interface IUnitOfWork
    {
        IBlogRepository BlogRepository { get; }
        ICommentRepository CommentRepository { get; }
        DbContext Context { get; }
        Task CompleteAsync();
    }
}