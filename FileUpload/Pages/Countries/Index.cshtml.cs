using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using FileUpload.Models;

namespace FileUpload.Pages.Countries
{
    public class IndexModel : PageModel
    {
        private readonly FileUpload.Models.FileUploadContext _context;

        public IndexModel(FileUpload.Models.FileUploadContext context)
        {
            _context = context;
        }

        public IList<Country> Country { get;set; }

        public async Task OnGetAsync()
        {
            Country = await _context.Country.ToListAsync();
        }
    }
}
