using ContractorsDesk.Core.Enums;
using ContractorsDesk.WebPortal.Models;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Text.Json;
using ContractorsDesk.Services.Interfaces;
using ContractorsDesk.Core.Models;

namespace ContractorsDesk.WebPortal.Controllers.APIControllers
{
	public abstract class BaseApiController : ControllerBase
	{
		public readonly string tempDirectory = Path.Combine(Path.GetTempPath(), "UploadedFiles");
		private readonly IHttpContextAccessor httpContextAccessor;
        public IPermissionsService permissionsService { get; set; }
		public IApplicationUserService applicationUserService { get; set; }
		public IAzureStorageService? azureStorageService { get; set; }
		public PermissionViewModel Permissions { get; set; } = new PermissionViewModel();

		public int UserId { get; set; }
		public int RoleId { get; set; }
        public int? CompanyId { get; set; }
        public int RoleCategoryId { get; set; }
        public List<string> UserPermissions {get;set;}
		public ApplicationUserModel CurrentUser { get; set; }
		public Roles? CurrentRole { get; set; }

		public BaseApiController() { }
		public BaseApiController(
			IHttpContextAccessor httpContextAccessor, 
			IPermissionsService permissionsService, 
			IApplicationUserService applicationUserService,
			IAzureStorageService? azureStorageService = null)
		{
			this.httpContextAccessor = httpContextAccessor;
			this.permissionsService = permissionsService;
			this.applicationUserService = applicationUserService;
			this.azureStorageService = azureStorageService;
            var httpContext = httpContextAccessor.HttpContext;
            var userSession = httpContext?.User;
			var appUserClaim = userSession?.FindFirst("ApplicationUser")?.Value;
			
			if (!string.IsNullOrEmpty(appUserClaim))
			{
				var appUser = JsonSerializer.Deserialize<ApplicationUserModel>(appUserClaim);

				if (appUser is null || appUser.RoleCategoryId == 0)
					throw new Exception("401");


				if (appUser != null)
				{
					this.UserId = Convert.ToInt32(userSession?.FindFirst(ClaimTypes.NameIdentifier)?.Value);
					this.CurrentUser = appUser;
					this.RoleId = appUser.RoleId;
					this.CompanyId = appUser.CompanyId;
					this.RoleCategoryId = appUser.RoleCategoryId;
					this.CurrentRole = GetRole(this.RoleId);

                    var userPermissions = httpContext?.Request.Cookies["contractorsdesk_permissions"];

					this.UserPermissions = JsonSerializer.Deserialize<List<string>>(userPermissions);

					SetPermissions();
				}
            }
        }

		[NonAction]
		public async Task<List<AttachmentLinkModel>> GetAttachmentsFromTempFolder(EmailPayloadModel emailModel)
		{
			var retval = new List<AttachmentLinkModel>();
			if (emailModel.AttachmentListId != null)
			{
				var attachmentListId = emailModel.AttachmentListId;
				var files = this.GetFilesFromTempDirectory(attachmentListId);
				if (files != null && files.Any())
				{
					string formattedDateTime = DateTime.Now.ToString("dd-MM-yyyy");
					foreach (var file in files)
					{
						var fileName = file.FileName;
						var fileUrl = await azureStorageService.UploadFile(file, "email-attachments", fileName, $"{formattedDateTime}/{attachmentListId}");
						var emailAttachment = new AttachmentLinkModel
						{
							FileName = fileName,
							FileUrl = fileUrl
						};

						retval.Add(emailAttachment);
					}

					CleanupTempDirectories();
				}
			}
			return retval;
		}

		[NonAction]
		public async Task SaveFileToTempDirectory(IFormFile file, string id)
		{
			var directoryPath = Path.Combine(tempDirectory, id);
			if (!Directory.Exists(directoryPath))
			{
				Directory.CreateDirectory(directoryPath);
			}

			var filePath = Path.Combine(directoryPath, file.FileName);
			using (var stream = new FileStream(filePath, FileMode.Create))
			{
				await file.CopyToAsync(stream);
			}
		}

		[NonAction]
		public List<IFormFile> GetFilesFromTempDirectory(string id)
		{
			var directoryPath = Path.Combine(tempDirectory, id);
			var files = new List<IFormFile>();

			if (Directory.Exists(directoryPath))
			{
				var filePaths = Directory.GetFiles(directoryPath);
				foreach (var filePath in filePaths)
				{
					var stream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.None);
					var formFile = new FormFile(stream, 0, stream.Length, null, Path.GetFileName(filePath))
					{
						Headers = new HeaderDictionary(),
						ContentType = "application/octet-stream"
					};

					files.Add(formFile);
				}
			}

			return files;
		}

		[NonAction]
		public void CleanupTempDirectories()
		{
			if (Directory.Exists(tempDirectory))
			{
				var subDirectories = Directory.GetDirectories(tempDirectory);
				foreach (var directories in subDirectories)
				{
					try
					{
						Directory.Delete(tempDirectory, true);
					}
					catch (IOException) { }
				}
			}
		}

		#region Private
		[NonAction]
		private Roles? GetRole(int roleId)
		{
            if (Enum.IsDefined(typeof(Roles), roleId))
            {
                return (Roles)roleId;
            }
            else
            {
                return null;
            }
        }
		[NonAction]
		private void SetPermissions()
		{
			this.Permissions.CanManageAllJobs = this.HasPermission("Manage All Jobs");
			this.Permissions.CanManageOwnJobs = this.HasPermission("Manage Own Jobs");
			this.Permissions.CanManageAllActionItems = this.HasPermission("Manage All Action Items");
			this.Permissions.CanManageOwnActionItems = this.HasPermission("Manage Own Action Items");
			this.Permissions.CanManageCompanyUsers = this.HasPermission("Manage Company Users");
			this.Permissions.CanManageCompanyRoles = this.HasPermission("Manage Company Roles");
			this.Permissions.CanManageSystemUsers = this.HasPermission("Manage System Users");
			this.Permissions.CanManageSystemRoles = this.HasPermission("Manage System Roles");
			this.Permissions.CanManageOwnEstimates = this.HasPermission("Manage Own Estimates");
			this.Permissions.CanManageAllEstimates = this.HasPermission("Manage All Estimates");
			this.Permissions.CanAssignJobs = this.HasPermission("Can Assign Jobs");
			this.Permissions.CanAssignEstimates = this.HasPermission("Can Assign Estimates");
			this.Permissions.CanAssignActionItems = this.HasPermission("Can Assign Action Items");
			this.Permissions.CanAccessClientJobs = this.HasPermission("Can Access Client Jobs");
			this.Permissions.CanManageDataMapping = this.HasPermission("Can Manage Data Mapping");
            this.Permissions.CanEditAcceptedProposals = this.HasPermission("Can Edit Accepted Proposals");
            this.Permissions.CanEditCompletedSchedules = this.HasPermission("Can Edit Completed Schedules");
            this.Permissions.CanAccessCompanySettings = this.HasPermission("Can Access Company Settings");
			this.Permissions.CanAccessDataSyncServices = this.HasPermission("Can Access Data Sync Services");
        }
		[NonAction]
		private bool HasPermission(string permission)
		{
            return this.UserPermissions.Any(p => string.Equals(p, permission, StringComparison.OrdinalIgnoreCase));
        }

		[NonAction]
		public string GetRootUrl()
		{
			var request = HttpContext.Request;
			return $"{request.Scheme}://{request.Host}";
		}

		#endregion
	}
}
