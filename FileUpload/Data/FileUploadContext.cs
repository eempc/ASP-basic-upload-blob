using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using FileUpload.Models;

namespace FileUpload.Models
{
    public class FileUploadContext : DbContext
    {
        public FileUploadContext (DbContextOptions<FileUploadContext> options)
            : base(options)
        {
        }

        public DbSet<FileUpload.Models.Country> Country { get; set; }

        public DbSet<FileUpload.Models.Cat> Cat { get; set; }
    }
}
