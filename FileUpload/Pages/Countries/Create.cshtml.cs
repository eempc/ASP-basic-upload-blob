using Azure.Storage.Blobs;
using FileUpload.Models;
using FileUpload.Protected;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.IO;
using System.Threading.Tasks;

namespace FileUpload.Pages.Countries {
    public class CreateModel : PageModel {
        private readonly FileUpload.Models.FileUploadContext _context;
        public IWebHostEnvironment _environment { get; set; }

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
        public IFormFile FormFileToBeUploaded { get; set; } // The upload has to be a separate property

        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync() {
            //string rootPath = Path.Combine(_environment.ContentRootPath, "Uploads");

            //if (!Directory.Exists(rootPath)) {
            //    Directory.CreateDirectory(Path.Combine(rootPath));
            //}

            //string filePath = Path.Combine(rootPath, Country.FormFile.FileName);

            Country.FileName = "Test" + FormFileToBeUploaded.FileName; // Can extract the name, but can I turn a FormFile into a FileStream?

            if (!ModelState.IsValid) {
                return Page();
            }

            _context.Country.Add(Country);
            await _context.SaveChangesAsync();

            //BlobContainerClient containerClient = new BlobContainerClient(Secrets.connectionString, "images");
            BlobClient blobClient = new BlobClient(connectionString: Secrets.connectionString, blobContainerName: "images", blobName: Country.FileName);

            using Stream uploadFileStream = FormFileToBeUploaded.OpenReadStream();
            await blobClient.UploadAsync(uploadFileStream);
            uploadFileStream.Close();

            //using (FileStream fs = new FileStream(filePath, FileMode.Create)) {
            //    await Country.FormFile.CopyToAsync(fs);
            //}

            return RedirectToPage("./Index");
        }
    }
}
