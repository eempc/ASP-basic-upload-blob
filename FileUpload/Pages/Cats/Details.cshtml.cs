using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using FileUpload.Models;

namespace FileUpload.Pages.Cats
{
    public class DetailsModel : PageModel
    {
        private readonly FileUpload.Models.FileUploadContext _context;

        public DetailsModel(FileUpload.Models.FileUploadContext context)
        {
            _context = context;
        }

        public Cat Cat { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Cat = await _context.Cat.FirstOrDefaultAsync(m => m.Id == id);

            if (Cat == null)
            {
                return NotFound();
            }
            return Page();
        }
    }
}
