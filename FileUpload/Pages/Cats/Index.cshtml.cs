using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using FileUpload.Models;
using FileUpload.Protected;
using Microsoft.Azure.Storage.Blob;
using Microsoft.Azure.Storage.Auth;

namespace FileUpload.Pages.Cats
{
    public class IndexModel : PageModel
    {
        private readonly FileUpload.Models.FileUploadContext _context;

        public List<string> imageUrls = new List<string>();

        public IndexModel(FileUpload.Models.FileUploadContext context)
        {
            _context = context;
        }

        public IList<Cat> Cat { get;set; }

        public async Task OnGetAsync()
        {
            Cat = await _context.Cat.ToListAsync();

            Uri containerUrl = new Uri(Secrets.thumbContainer2); // Must be a public container to download
            StorageCredentials cred = new StorageCredentials(Secrets.storageName2, Secrets.storageKey2);

            CloudBlobContainer container = new CloudBlobContainer(containerUrl, cred);
            BlobContinuationToken token = null;
            BlobResultSegment resultSegment = null;

            string prefixCheck = "Testingt"; // Get only file names beginning with "Testingt"

            do {
                resultSegment = await container.ListBlobsSegmentedAsync(prefixCheck, true, BlobListingDetails.All, 10, token, null, null);

                foreach (var item in resultSegment.Results) {
                    imageUrls.Add(item.StorageUri.PrimaryUri.ToString());
                }

                token = resultSegment.ContinuationToken;
            } while (token != null);         
        }
    }
}
