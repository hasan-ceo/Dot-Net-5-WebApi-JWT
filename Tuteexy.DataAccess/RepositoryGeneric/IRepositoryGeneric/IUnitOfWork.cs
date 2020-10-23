using System;

namespace Tuteexy.Service
{
    public interface IUnitOfWork : IDisposable
    {
        IUserRefreshTokenRepository UserRefreshToken { get; }
        void Save();
    }
}
