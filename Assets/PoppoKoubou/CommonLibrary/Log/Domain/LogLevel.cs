using System;

namespace PoppoKoubou.CommonLibrary.Log.Domain
{
    [Flags] public enum LogLevel
    {
        None    = 0,                // 何も設定されていない状態
        Verbose = 1 << 0,           // 1
        Debug   = 1 << 1,           // 2
        Info    = 1 << 2,           // 4
        Warning = 1 << 3,           // 8
        Error   = 1 << 4,           // 16
        Fatal   = 1 << 5,           // 32
        All     = Verbose | Debug | Info | Warning | Error | Fatal, // すべてのフラグを組み合わせたもの
        Default = Info | Warning | Error | Fatal  // 標準的なフラグを組み合わせたもの
    }
}