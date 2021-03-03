using Pjfm.Application.AppContexts.Users;
using Pjfm.Domain.Entities;

namespace Pjfm.Infrastructure.Service
{
    public static class ApplicationUserSerializer
    {
        public static ApplicationUserDto SerializeToDto(ApplicationUser user)
        {
            return new ApplicationUserDto()
            {
                Id = user.Id,
                DisplayName = user.DisplayName,
            };
        }
    }
}