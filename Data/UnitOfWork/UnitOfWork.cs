using SimpleBlogMVC.Data.Repositories.Interfaces;
using System.Threading.Tasks;

namespace SimpleBlogMVC.Data
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _context;
        public IBlogRepository BlogRepository { get; private set; }

        public UnitOfWork(ApplicationDbContext context, IBlogRepository blogRepository)
        {
            _context = context;
            BlogRepository = blogRepository;
        }

        public async Task CompleteAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
