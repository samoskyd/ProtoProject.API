using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Microsoft.EntityFrameworkCore;
using ProtoProject.API.Models;
using ProtoProject.API.Data;
using System.Drawing;
using Microsoft.AspNetCore.Authorization;
using static NuGet.Packaging.PackagingConstants;

namespace ProtoProject.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class FolderController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly DataContext _context;
        private readonly BlobStorageService _blobStorageService;

        public FolderController(IConfiguration configuration, DataContext context)
        {
            _blobStorageService = new BlobStorageService(configuration);
            _configuration = configuration;
            _context = context;
        }

        [HttpPost("uploadFile/{cardId}/{folderType}")]
        public async Task<IActionResult> UploadFile(int cardId, string folderType, IFormFile file)
        {
            var card = await _context.Cards
                .Include(c => c.LinkFolder)
                .Include(c => c.ImageFolder)
                .Include(c => c.DocFolder)
                .FirstOrDefaultAsync(c => c.CardId == cardId);

            if (card == null)
            {
                return NotFound();
            }
            var containerName = _configuration["AzureStorage:ContainerName"];
            LinkFolder lf = card.LinkFolder;
            ImageFolder imf = card.ImageFolder;
            DocFolder df = card.DocFolder;
            switch (folderType)
            {
                case "link":
                    containerName = "links";
                    break;
                case "image":
                    containerName = "images";
                    break;
                case "doc":
                    containerName = "docs";
                    break;
                default:
                    return BadRequest("Invalid folder type 1.");
            }

            var blobName = GenerateUniqueBlobName(file.FileName);
            await _blobStorageService.UploadFileAsync(containerName, blobName, file.OpenReadStream());

            switch (folderType)
            {
                case "link":
                    lf.ContainerName = containerName;
                    lf.BlobName = blobName;
                    break;
                case "image":
                    imf.ContainerName = containerName;
                    imf.BlobName = blobName;
                    break;
                case "doc":
                    df.ContainerName = containerName;
                    df.BlobName = blobName;
                    break;
                default:
                    return BadRequest("Invalid folder type 3.");
            }

            await _context.SaveChangesAsync();

            switch (folderType)
            {
                case "link":
                    return Ok(lf);
                    break;
                case "image":
                    return Ok(imf);
                    break;
                case "doc":
                    return Ok(df);
                    break;
                default:
                    return BadRequest();
            }
        }

        [HttpGet("download/{folderType}/{folderId}")]
        public async Task<IActionResult> DownloadFile(string folderType, int folderId)
        {
            try
            {
                string containerName = string.Empty;
                string blobName = string.Empty;

                switch (folderType)
                {
                    case "link":
                        LinkFolder lfolder = await _context.LinkFolders.FindAsync(folderId);
                        containerName = "links";
                        blobName = lfolder.BlobName;
                        if (lfolder == null)
                        {
                            return NotFound();
                        }
                        break;
                    case "image":
                        ImageFolder ifolder = await _context.ImageFolders.FindAsync(folderId);
                        containerName = "images";
                        blobName = ifolder.BlobName;
                        if (ifolder == null)
                        {
                            return NotFound();
                        }
                        break;
                    case "doc":
                        DocFolder dfolder = await _context.DocFolders.FindAsync(folderId);
                        containerName = "docs";
                        blobName = dfolder.BlobName;
                        if (dfolder == null)
                        {
                            return NotFound();
                        }
                        break;
                }

                if (containerName == string.Empty)
                {
                    return BadRequest($"folder type not valid. folder type: {folderType}");
                }

                var stream = await _blobStorageService.DownloadFileAsync(containerName, blobName);
                return File(stream, "application/octet-stream", blobName);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred while downloading the file: {ex.Message}");
            }
        }


        [HttpDelete("delete/{folderType}/{folderId}")]
        public async Task<IActionResult> DeleteFile(string folderType, int folderId)
        {
            try
            {
                string containerName = string.Empty;
                string blobName = string.Empty;

                switch (folderType)
                {
                    case "link":
                        LinkFolder lfolder = await _context.LinkFolders.FindAsync(folderId);
                        containerName = "links";
                        blobName = lfolder.BlobName;
                        if (lfolder == null)
                        {
                            return NotFound();
                        }
                        break;
                    case "image":
                        ImageFolder ifolder = await _context.ImageFolders.FindAsync(folderId);
                        containerName = "images";
                        blobName = ifolder.BlobName;
                        if (ifolder == null)
                        {
                            return NotFound();
                        }
                        break;
                    case "doc":
                        DocFolder dfolder = await _context.DocFolders.FindAsync(folderId);
                        containerName = "docs";
                        blobName = dfolder.BlobName;
                        if (dfolder == null)
                        {
                            return NotFound();
                        }
                        break;
                }

                if (containerName == string.Empty)
                {
                    return BadRequest($"folder type not valid. folder type: {folderType}");
                }

                await _blobStorageService.DeleteFileAsync(containerName, blobName);
                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred while deleting the file: {ex.Message}");
            }
        }

        private string GenerateUniqueBlobName(string fileName)
        {
            var uniqueName = Guid.NewGuid().ToString("N");
            var extension = Path.GetExtension(fileName);
            return $"{uniqueName}{extension}";
        }
    }
}
