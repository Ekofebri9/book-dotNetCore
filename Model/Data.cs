using basicwebapi.Model.Entity;
using Microsoft.EntityFrameworkCore;
namespace basicwebapi.Model
{
    public class Data : DbContext
    {
        public Data(DbContextOptions<Data>options)
        : base(options)
        {

        }
        public DbSet<Book> Books {get; set;}
    }
}