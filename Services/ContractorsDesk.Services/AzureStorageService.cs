using AutoMapper;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using ContractorsDesk.DataStore.Client.Models;
using ContractorsDesk.DataStore.Master.Models;
using ContractorsDesk.Services.@base;
using ContractorsDesk.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.EntityFrameworkCore;
using System.IO;

namespace ContractorsDesk.Services
{
	public class AzureStorageService : BaseService, IAzureStorageService
	{
		private readonly BlobServiceClient blobServiceClient;
		private string CompanyName;
		public AzureStorageService(
			IMapper mapper,
			ClientDbContext clientDbContext,
			MasterDbContext masterDbContext,
			BlobServiceClient blobServiceClient) : base(mapper: mapper, clientDataDbContext: clientDbContext, masterDbContext: masterDbContext)
		{
			this.blobServiceClient = blobServiceClient;
		}

		public async Task<string> UploadFile(IFormFile file, string containerName, string fileName, string directoryName = "")
		{
			if (file == null || file.Length == 0)
			{
				throw new Exception("No file was uploaded.");
			}

			try
			{
				await using (var stream = file.OpenReadStream())
				{
					return await this.UploadFileStream(stream, containerName, fileName, directoryName);
				}
			}
			catch (Exception ex)
			{
				throw new Exception(ex.Message);
			}
		}
		public async Task<string> UploadFileFromStream(Stream fileStream, string containerName, string fileName, string directoryName = "")
		{
			if (fileStream == null || fileStream.Length == 0)
			{
				throw new Exception("No file was uploaded.");
			}

			try
			{
				return await this.UploadFileStream(fileStream, containerName, fileName, directoryName);
			}
			catch (Exception ex)
			{
				throw new Exception(ex.Message);
			}
		}
		public async Task<string> UploadFileFromBase64(string base64Str, string containerName, string fileName, string directoryName = "")
		{
			try
			{
				byte[] fileBytes = Convert.FromBase64String(base64Str);
				using var stream = new MemoryStream(fileBytes);

				return await this.UploadFileStream(stream, containerName, fileName, directoryName);
			}
			catch (Exception ex)
			{
				throw new Exception(ex.Message);
			}
		}
		public async Task<bool> DeleteFileAsync(string blobUrl)
        {
			var blobName = GetBlobNameFromUrl(blobUrl);

			var blobContainerClient = await GetBlobContainerClient();
            var blobClient = blobContainerClient.GetBlobClient(blobName);

            var response = await blobClient.DeleteIfExistsAsync();
            return response.Value; // true if deleted, false if blob didn't exist
        }
		private async Task<string> UploadFileStream(Stream fileStream, string containerName, string fileName, string directoryName = "")
		{

			// Get a reference to the container
			//var blobContainerClient = blobServiceClient.GetBlobContainerClient(containerName);
			var blobContainerClient = await GetBlobContainerClient();

			// Ensure the container exists
			//await blobContainerClient.CreateIfNotExistsAsync();

			// Combine directory name and file name to create the full path
			var blobName = string.IsNullOrEmpty(directoryName) ? fileName : $"{containerName}/{directoryName}/{fileName}";

			// Get a reference to the blob
			var blobClient = blobContainerClient.GetBlobClient(blobName);

			var provider = new FileExtensionContentTypeProvider();
			if (!provider.TryGetContentType(fileName, out string contentType))
			{
				contentType = "application/octet-stream";
			}
			// Upload with headers
			var options = new BlobUploadOptions
			{
				// Define headers
				HttpHeaders = new BlobHttpHeaders
				{
					ContentType = contentType,
					ContentDisposition = "inline"
				}
			};

			// Upload the file
			await blobClient.UploadAsync(fileStream, options);

			return blobClient.Uri.ToString();
		}
        private async Task<BlobContainerClient> GetBlobContainerClient()
		{
			var companySetting = await ClientDbContext.CompanySettings.FirstOrDefaultAsync();
			if(companySetting == null) throw new Exception("Company settings not found.");

			this.CompanyName = companySetting.CompanyName;
			var containerName = this.CompanyName; 
            containerName = string.Concat(containerName.Where(c => !char.IsWhiteSpace(c)));
            containerName = containerName.Replace("-", "");
			containerName = containerName.ToLowerInvariant();

            var blobContainerClient = blobServiceClient.GetBlobContainerClient(containerName);

            if (!await blobContainerClient.ExistsAsync())
            {
                await blobContainerClient.CreateAsync(PublicAccessType.Blob);
            }

			return blobContainerClient;
        }
		private static string GetBlobNameFromUrl(string blobUrl)
		{
			var uri = new Uri(blobUrl);
			var fullPath = uri.AbsolutePath.TrimStart('/');

			// Remove the container name (first segment)
			var firstSlashIndex = fullPath.IndexOf('/');
			if (firstSlashIndex < 0)
				throw new ArgumentException("Invalid blob URL format.");

			var relativePath = fullPath.Substring(firstSlashIndex + 1);

			return Uri.UnescapeDataString(relativePath);

		}
	}
}
