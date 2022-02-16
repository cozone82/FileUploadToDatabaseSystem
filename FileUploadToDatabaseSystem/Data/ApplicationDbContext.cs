using FileUploadToDatabaseSystem.Models;
using Microsoft.EntityFrameworkCore;

namespace FileUploadToDatabaseSystem.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {

        }
    
        
        public DbSet<FileOnDatabaseModel> FilesOnDatabase { get; set; }

    }
}
