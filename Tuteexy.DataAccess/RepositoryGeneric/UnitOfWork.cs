using Microsoft.AspNetCore.Identity;
using Tuteexy.Models;

namespace Tuteexy.Service
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _db;

        public UnitOfWork(ApplicationDbContext db)
        {
            _db = db;

            #region Idt
            UserRefreshToken = new UserRefreshTokenRepository(_db);
            #endregion

            #region Hub

            #endregion

            #region Lms

            #endregion

            #region Acct

            #endregion

            #region Hrm

            #endregion

        }


        #region Idt
        public IUserRefreshTokenRepository UserRefreshToken { get; private set; }
        #endregion


        #region Hub

        #endregion

        #region Lms

        #endregion

        #region Acct

        #endregion

        #region Hrm

        #endregion

        public void Dispose()
        {
            _db.Dispose();
        }

        public void Save()
        {
            _db.SaveChanges();
        }
    }
}
