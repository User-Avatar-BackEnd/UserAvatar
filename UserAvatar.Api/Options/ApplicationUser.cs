using System.Linq;
using Microsoft.AspNetCore.Http;

namespace UserAvatar.Api.Options
{
    public class ApplicationUser: IApplicationUser
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public ApplicationUser(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public int Id => GetUserId();
        
        private int GetUserId()
        {
            var request = _httpContextAccessor.HttpContext
                ?.User.Claims.FirstOrDefault(x => x.Type == "id");

            return int.TryParse(request?.Value, out var id) ? id : 0;
        }
        
    }
}