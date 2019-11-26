using Azure.Storage.Blobs;
using FileUpload.Models;
using FileUpload.Protected;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.IO;
using System.Threading.Tasks;

namespace FileUpload.Pages.Countries {
    public class CreateModel : PageModel {
        private readonly FileUpload.Models.FileUploadContext _context;

        public CreateModel(FileUpload.Models.FileUploadContext context) {
            _context = context;
        }

        public IActionResult OnGet() {
            return Page();
        }

        [BindProperty]
        public Country Country { get; set; } // The model
        [BindProperty]
        public IFormFile FormFileToBeUploaded { get; set; } // The uploaded file has to be a separate property

        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync() {
            //This is the important step to make a file identifiable
            Country.FileName = "Test" + FormFileToBeUploaded.FileName; // A simple test to see if I can extract the name of the uploaded file and apply it to the model's FileName
            //Need to also remember a container name
            //https://github.com/Azure/azure-sdk-for-net/blob/6e51ae9d74b2afebd02b253eaf82b18082a78da9/sdk/storage/Azure.Storage.Blobs/README.md

            if (!ModelState.IsValid) {
                return Page();
            }

            _context.Country.Add(Country);
            await _context.SaveChangesAsync();

            // In production, create a new container for each person who uploads, or each new listing, and remember this name for each listing/person
            //BlobServiceClient blobServiceClient = new BlobServiceClient(Secrets.connectionString);
            //string containerName = "UniqueContainerName"; // Give it a name: "MyContainer1" could be specific to a user, which it probably should
            //(BlobContainerClient containerClient =) await blobServiceClient.CreateBlobContainerAsync(containerName);

            //BlobContainerClient containerClient = new BlobContainerClient(Secrets.connectionString, "images");
            BlobClient blobClient = new BlobClient(connectionString: Secrets.connString2, blobContainerName: "thumbnails", blobName: Country.FileName);

            //FormFileToBeUploaded.ContentType = "image/png";

            using Stream uploadFileStream = FormFileToBeUploaded.OpenReadStream();
            await blobClient.UploadAsync(uploadFileStream);
            uploadFileStream.Close();

            return RedirectToPage("./Index");
        }
    }
}
