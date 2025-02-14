using System;

namespace PoppoKoubou.CommonLibrary.Log.Domain
{
    /// <summary>ログプロバイダ</summary>
    public interface ILogProvider : IDisposable
    {
        void Initialize();
    }
}