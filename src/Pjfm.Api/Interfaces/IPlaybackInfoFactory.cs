using pjfm.Models;

namespace pjfm.Interfaces
{
    public interface IPlaybackInfoFactory
    {
        UserPlaybackInfoModel CreateUserInfoModel();
        DjPlaybackInfoModel CreateDjInfoModel();
    }
}