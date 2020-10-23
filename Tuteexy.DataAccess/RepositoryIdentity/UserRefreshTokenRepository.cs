using System.Linq;
using Tuteexy.Models;

namespace Tuteexy.Service
{
    public class UserRefreshTokenRepository : RepositoryAsync<UserRefreshToken>, IUserRefreshTokenRepository
    {
        private readonly ApplicationDbContext _db;

        public UserRefreshTokenRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

        public void Update(UserRefreshToken data)
        {
            var objFromDb = _db.UserRefreshTokens.FirstOrDefault(s => s.UserID == data.UserID);
            if (objFromDb != null)
            {
                objFromDb.RefreshToken = data.RefreshToken;
                objFromDb.ExpiryDate = data.ExpiryDate;
            }
        }

    }

}
