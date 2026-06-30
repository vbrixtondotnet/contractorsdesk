namespace ContractorsDesk.Core.Dto
{
	public class ApplicationUserDto
	{
		public int Id { get; set; }
		public required string FirstName { get; set; }
		public required string LastName { get; set; }
		public DateTime? CreatedDate { get; set; }
		public string Email { get; set; }
		public bool EmailConfirmed { get; set; }
		public bool RequireLogOn { get; set; }
		public List<RoleDto> Roles { get; set; }
		public string CreatedDateString { get { return this.CreatedDate?.ToShortDateString(); } }
		public string FullName { get { return $"{this.FirstName} {this.LastName}"; } }
		public string Role { get; set; } = string.Empty;
		public int? RoleId { get; set; } = 0;
		public int? CompanyId { get; set; }
		public int? SupervisorTypeId { get; set; }
		private string? _avatarUrl;
		public string? AvatarUrl
		{
			get
			{
				return this._avatarUrl ?? "/assets/media/avatars/blank.png";
			}
			set { this._avatarUrl = value; }
		}
		public string Initials
		{
			get
			{
				var firstInitial = !string.IsNullOrEmpty(FirstName) ? FirstName[0].ToString().ToUpper() : "";
				var lastInitial = !string.IsNullOrEmpty(LastName) ? LastName[0].ToString().ToUpper() : "";
				return firstInitial + lastInitial;
			}
		}

	}

    public class ApplicationUserShortDetailsDto
    {
        public int Id { get; set; }
        public required string FirstName { get; set; }
        public required string LastName { get; set; }
		public string Email { get; set; } = string.Empty;
		public string Initials
		{
			get
			{
				var firstInitial = !string.IsNullOrEmpty(FirstName) ? FirstName[0].ToString().ToUpper() : "";
				var lastInitial = !string.IsNullOrEmpty(LastName) ? LastName[0].ToString().ToUpper() : "";
				return firstInitial + lastInitial;
			}
		}
	}
}
