﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using FileUpload.Models;
using Microsoft.AspNetCore.Http;
using FileUpload.Protected;
using Microsoft.Azure.Storage.Auth;
using Microsoft.Azure.Storage.Blob;
using System.IO;

namespace FileUpload.Pages.Cats
{
    public class CreateModel : PageModel
    {
        private readonly FileUpload.Models.FileUploadContext _context;

        public CreateModel(FileUpload.Models.FileUploadContext context)
        {
            _context = context;
        }

        public IActionResult OnGet()
        {
            return Page();
        }

        [BindProperty]
        public Cat Cat { get; set; }
        // 1 Create FormFile
        [BindProperty]
        public IFormFileCollection FormFileToBeUploaded { get; set; }
        

        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.
        // https://docs.microsoft.com/en-us/azure/storage/blobs/storage-upload-process-images?tabs=dotnet
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            _context.Cat.Add(Cat);
            await _context.SaveChangesAsync();

            foreach (var item in FormFileToBeUploaded) {
                using (Stream stream = item.OpenReadStream()) {
                    StorageCredentials cred = new StorageCredentials(Secrets.storageName2, Secrets.storageKey2);
                    Uri url = new Uri(Secrets.imageContainer2);

                    // The following container was premade, but if it wasn't, there could be a check for if container exists
                    CloudBlobContainer container = new CloudBlobContainer(url, cred);
                    if (!container.Exists()) {
                        return RedirectToPage("./Index");
                        // Create the container?
                        // In production it would always be create container for the images belonging to that listing
                    }

                    string fileName = "Testing" + item.FileName;
                    CloudBlockBlob blockBlob = container.GetBlockBlobReference(fileName);
                    await blockBlob.UploadFromStreamAsync(stream);
                }
            }



            return RedirectToPage("./Index");
        }
    }
}
