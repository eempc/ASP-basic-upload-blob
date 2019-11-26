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
    public class DeleteModel : PageModel
    {
        private readonly FileUpload.Models.FileUploadContext _context;

        public DeleteModel(FileUpload.Models.FileUploadContext context)
        {
            _context = context;
        }

        [BindProperty]
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

        public async Task<IActionResult> OnPostAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Cat = await _context.Cat.FindAsync(id);

            if (Cat != null)
            {
                _context.Cat.Remove(Cat);
                await _context.SaveChangesAsync();
            }

            return RedirectToPage("./Index");
        }
    }
}
