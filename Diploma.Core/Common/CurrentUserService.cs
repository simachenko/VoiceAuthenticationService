using Diploma.Core.Common.Interfaces;
using Diploma.DTO.Authorization;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Diploma.Core.Common
{
	public class CurrentUserService : ICurrentUserService
	{
		private readonly IHttpContextAccessor _httpContextAccessor;
		private ClaimsPrincipal _httpUserContext => _httpContextAccessor.HttpContext?.User;
		public CurrentUser User { get; set; } = new CurrentUser();
		public CurrentUserService(IHttpContextAccessor httpContextAccessor)
		{
			_httpContextAccessor = httpContextAccessor;
			setUserDetails();
		}

		private void setUserDetails()
		{
			var familyId = _httpUserContext.Claims.FirstOrDefault(x => x.Type == nameof(User.FamilyId));
			if (familyId != null)
			{
				User.FamilyId = Guid.Parse(familyId.Value);
			}
			var userId = _httpUserContext.Claims.FirstOrDefault(x => x.Type == nameof(User.UserId));
			if (userId != null)
			{
				User.UserId = Guid.Parse(userId.Value);
			}
			var role = _httpUserContext.Claims.FirstOrDefault(x => x.Type == ClaimsIdentity.DefaultRoleClaimType);
			if (role != null)
			{
				User.Role = (UserRole)int.Parse(role.Value);
			}
			
		}

		public class CurrentUser
		{
			public Guid UserId { set; get; }
			public Guid FamilyId { set; get; }
			public UserRole Role { set; get; }
		}
	}
}
