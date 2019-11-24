using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using FileUpload.Models;
using Microsoft.AspNetCore.Hosting;
using System.IO;
using Microsoft.AspNetCore.Http;

namespace FileUpload.Pages.Countries {
    public class CreateModel : PageModel {
        private readonly FileUpload.Models.FileUploadContext _context;
        public IWebHostEnvironment _environment;

        public CreateModel(FileUpload.Models.FileUploadContext context, IWebHostEnvironment environment) {
            _context = context;
            _environment = environment;
        }

        public IActionResult OnGet() {
            return Page();
        }

        [BindProperty]
        public Country Country { get; set; }
        [BindProperty]
        public IFormFile Upload { get; set; } // The upload has to be a separate property

        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync() {
            //string rootPath = Path.Combine(_environment.ContentRootPath, "Uploads");

            //if (!Directory.Exists(rootPath)) {
            //    Directory.CreateDirectory(Path.Combine(rootPath));
            //}

            //string filePath = Path.Combine(rootPath, Country.FormFile.FileName);

            Country.FileName = "Test" + Upload.FileName; // Can extract the name, but can I turn a FormFile into a FileStream?

            if (!ModelState.IsValid) {
                return Page();
            }

            _context.Country.Add(Country);
            await _context.SaveChangesAsync();



            //using (FileStream fs = new FileStream(filePath, FileMode.Create)) {
            //    await Country.FormFile.CopyToAsync(fs);
            //}

            return RedirectToPage("./Index");
        }
    }
}
