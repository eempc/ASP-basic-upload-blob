# Very quick start guide to azure blob image storage and viewing of images by client browser

## Set up Azure

1. Create a storage account (can be done in portal or CLI)
	* Performance/Access tier: Standard/Hot  
	* Account kind: StorageV2 (general purpose)]
	* Make a note of connection string and key
2. Create two containers and record their direct URIs
	* "images" - Private access
	* "thumbnails" - Container access
	* E.g. direct URI https://<storage-name>.blob.core.windows.net/images which can be found in container properties via portal

Azure blob storage is ready to receive images

## Upload from a Razor page

1. Install Microsoft.Azure.Storage, not Azure.Storage

* .CS file

```
[BindProperty]
public IFormFile FormFileToBeUploaded { get; set; } // In the PageModel 

// The following can be sequestered away in a static class
using (Stream stream = FormFileToBeUploaded.OpenReadStream()) {
	// Get these value from storage access keys in portal (store them in Key Vault)
    StorageCredentials cred = new StorageCredentials(Secrets.storageName2, Secrets.storageKey2);
    // The direct URI from above
	Uri url = new Uri(Secrets.imageContainer2);

    // The following container was premade, but if it wasn't, there could be a check for if container exists
    CloudBlobContainer container = new CloudBlobContainer(url, cred);
    if (!container.Exists()) {
        //return RedirectToPage("./Index");
        // Create the container?
        // In production it would always be create container for the images belonging to that listing
    }

    string fileName = "Testing" + FormFileToBeUploaded.FileName;
    CloudBlockBlob blockBlob = container.GetBlockBlobReference(fileName);
    await blockBlob.UploadFromStreamAsync(stream);
}
```

* And the Form of the Razor page

```diff
        <form method="post" enctype="multipart/form-data">
            <div asp-validation-summary="ModelOnly" class="text-danger"></div>
            <div class="form-group">
                <label asp-for="Cat.Name" class="control-label"></label>
                <input asp-for="Cat.Name" class="form-control" />
                <span asp-validation-for="Cat.Name" class="text-danger"></span>
            </div>
 +          <input type="file" asp-for="FormFileToBeUploaded" />
            <div class="form-group">
                <input type="submit" value="Create" class="btn btn-primary" />
            </div>
        </form>
```

