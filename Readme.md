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
           <input type="file" asp-for="FormFileToBeUploaded" />
            <div class="form-group">
                <input type="submit" value="Create" class="btn btn-primary" />
            </div>
        </form>
```

## Resize the image and move the new thumbnail into the thumbnails folder

Unsure if this works with dynamically generated containers. If the function app can only apply to a single container then there are a couple of options:

* Have the web app resize the image on upload
* Create a separate storage container and get the function app to identify anything that goes in according to criteria

https://docs.microsoft.com/en-us/azure/event-grid/resize-images-on-storage-blob-upload-event?toc=%2Fazure%2Fstorage%2Fblobs%2Ftoc.json&tabs=dotnet

1. Create a Function App service either in Azure portal or CLI, e.g.

`az functionapp create --name <name-of-app> --storage-account <storage-account-name> --resource-group <Resource group name> --consumption-plan-location northeurope`

2. Configure the app, either in Azure portal or CLI

```
az functionapp config appsettings set --name <name-of-app> --resource-group <Resource group name>
--settings AzureWebJobsStorage=<storage-connection-string> THUMBNAIL_CONTAINER_NAME=thumbnails
THUMBNAIL_WIDTH=100 FUNCTIONS_EXTENSION_VERSION=~2
```

3. Deploy the resizing code (it's premade by MS) on https://github.com/Azure-Samples/function-image-upload-resize and means you must use the thumbnails and images container names

`az functionapp deployment source config --name $functionapp --resource-group $resourceGroupName --branch master --manual-integration --repo-url https://github.com/Azure-Samples/function-image-upload-resize`

4. Wait a few minutes
5. Create event subscription in Portal
6. Select your function app in Portal
7. Expand the Thumbnail function in Function (Read Only)
8. Click Add Event Grid Subscription
9. In Basic tab, give event a name, and choose Event Grid Schema. 
10. Select Topic type to be  storage account and topic resource is your storage account (the csa..... one)
11. Untick Subscribe to all event types and choose Blob Created in the next drop down
12. In Filters tab, tick the box Enable subject filtering
13. Subject Begins with: /blobServices/default/containers/images/blobs/
14. Create

## Download

* In CS

```
public List<string> urlOfImages = GetThumbnailUrls(); // can include arguments if you wish

public static async Task<List<string>> GetThumbnailUrls() {
	List<string> imageUrls = new List<string>();

	Uri containerUrl = new Uri(Secrets.thumbContainer2); // URL. Must be a public container to download
	StorageCredentials cred = new StorageCredentials(Secrets.storageName2, Secrets.storageKey2);

	CloudBlobContainer container = new CloudBlobContainer(containerUrl, cred);
	BlobContinuationToken token = null;
	BlobResultSegment resultSegment = null;

	do {
		resultSegment = await container.ListBlobsSegmentedAsync("", true, BlobListingDetails.All, 10, token, null, null);

		foreach (var item in resultSegment.Results) {
			imageUrls.Add(item.StorageUri.PrimaryUri.ToString());
		}

		token = resultSegment.ContinuationToken;
	} while (token != null);   

	// return imageUrls;

	return await Task.FromResult(imageUrls);
} 
```

* In razor page

```
@if (Model.urlOfImages.Count() > 0) {
    @foreach (string item in Model.urlOfImages) {
        <div>
            <p>@item.ToString() </p>
            <img src="@item" />
        </div>
    }
}
```