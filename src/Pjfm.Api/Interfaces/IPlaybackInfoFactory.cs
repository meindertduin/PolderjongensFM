using Pjfm.Api.Models;

namespace Pjfm.Api.Interfaces
{
    public interface IPlaybackInfoFactory
    {
        UserPlaybackInfoModel CreateUserInfoModel();
        DjPlaybackInfoModel CreateDjInfoModel();
    }
}