using Tuteexy.Models;

namespace Tuteexy.Service
{
    public interface IUserRefreshTokenRepository : IRepositoryAsync<UserRefreshToken>
    {
        void Update(UserRefreshToken data);
    }
}
