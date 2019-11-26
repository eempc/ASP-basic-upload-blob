using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using FileUpload.Models;
using Azure.Storage.Blobs.Models;
using Azure.Storage.Blobs;
using FileUpload.Protected;

// Did my best but it is best to switch to Microsoft.Azure.Storage.Blob because it has 3 million more dowloads than Azure.Storage.Blobs

namespace FileUpload.Pages.Countries {
    public class IndexModel : PageModel {
        private readonly FileUpload.Models.FileUploadContext _context;
        public List<BlobItem> blobList = new List<BlobItem>();

        public IndexModel(FileUpload.Models.FileUploadContext context) {
            _context = context;
        }

        public IList<Country> Country { get; set; }

        public List<string> imageUrls = new List<string>();

        public async Task OnGetAsync() {
            Country = await _context.Country.ToListAsync();
            BlobContainerClient blobClient = new BlobContainerClient(connectionString: Secrets.connectionString2, blobContainerName: "thumbnails");

            await foreach (BlobItem item in blobClient.GetBlobsAsync()) {
                
                blobList.Add(item);
            }

           
                
        }
    }
}

