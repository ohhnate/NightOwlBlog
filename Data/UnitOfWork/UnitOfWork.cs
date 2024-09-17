using SimpleBlogMVC.Data.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace SimpleBlogMVC.Data
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _context;
        public IBlogRepository BlogRepository { get; private set; }
        public ICommentRepository CommentRepository { get; private set; }

        public DbContext Context => _context;

        public UnitOfWork(ApplicationDbContext context, IBlogRepository blogRepository, ICommentRepository commentRepository)
        {
            _context = context;
            BlogRepository = blogRepository;
            CommentRepository = commentRepository;
        }

        public async Task CompleteAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}