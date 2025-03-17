using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using MessagePipe;
using PoppoKoubou.CommonLibrary.AggregateService.Domain;
using PoppoKoubou.CommonLibrary.Log.Domain;
using PoppoKoubou.CommonLibrary.Log.Infrastructure;
using VContainer;

// ReSharper disable InconsistentNaming
// ReSharper disable MemberCanBePrivate.Global

namespace PoppoKoubou.CommonLibrary.AggregateService.Infrastructure
{
    /// <summary>ノードサービス</summary>
    public abstract class ServiceNode : IServiceNode, IDisposable
    {
        /// <summary>ログ送信用Publisher</summary>
        protected readonly IPublisher<LogMessage> LogPublisher;

        /// <summary>サービス集約ハブステータス受信用Subscriber</summary>
        private readonly ISubscriber<CentralHubStatus> _centralHubStatusSubscriber;

        /// <summary>サービスノードステータス送信用Publisher</summary>
        private readonly IPublisher<ServiceNodeStatus> _serviceNodeStatusPublisher;

        /// <summary>依存注入</summary>
        [Inject] protected ServiceNode(
            string name,
            int priority,
            IPublisher<LogMessage> logPublisher,
            ISubscriber<CentralHubStatus> centralHubStatusSubscriber,
            IPublisher<ServiceNodeStatus> serviceNodeStatusPublisher)
        {
            serviceNodeInfo = new ServiceNodeInfo(name, priority);
            LogPublisher = logPublisher;
            _centralHubStatusSubscriber = centralHubStatusSubscriber;
            _serviceNodeStatusPublisher = serviceNodeStatusPublisher;
        }

        /// <summary>カラー開始</summary>
        protected readonly string LogColor = "#00ff00";
        protected readonly string ServiceLogColor = "#00ffff";

        /// <summary>サービスノード情報</summary>
        public ServiceNodeInfo ServiceNodeInfo => serviceNodeInfo;
        protected ServiceNodeInfo serviceNodeInfo;
        
        /// <summary>ノードサービス起動</summary>
        public async UniTask StartAsync(CancellationToken ct)
        {
            // 0.1秒待機
            await UniTask.Delay(TimeSpan.FromMilliseconds(100), cancellationToken: ct);
            // サービスノード登録開始待ち
            LogPublisher.Info($"[{serviceNodeInfo.Name}] サービスノード登録開始待ち ({serviceNodeInfo.Priority})");
            await _centralHubStatusSubscriber.FirstAsync(
                ct,
                ev => ev.Phase == CentralHubStatusPhase.WaitingRegistrationServiceNode);
            // 0.1秒待機
            await UniTask.Delay(TimeSpan.FromMilliseconds(100), cancellationToken: ct);
            // サービスノード登録
            LogPublisher.Info($"[{serviceNodeInfo.Name}] サービスノード登録 ({serviceNodeInfo.Priority})");
            _serviceNodeStatusPublisher.Publish(ServiceNodeStatus.RegistrationServiceNode(serviceNodeInfo));
            // サービスノード初期化開始待ち
            LogPublisher.Info($"[{serviceNodeInfo.Name}] サービスノード初期化開始待ち ({serviceNodeInfo.Priority})");
            var status = await _centralHubStatusSubscriber.FirstAsync(
                ct, 
                ev => ev.Phase == CentralHubStatusPhase.WaitingInitializeServiceNode &&
                      ev.Priority >= serviceNodeInfo.Priority);
            LogPublisher.Info($"[{serviceNodeInfo.Name}] セントラルハブステータス {status.Phase} {status.Priority}", "#4080ff");
            // サービスノード初期化開始
            LogPublisher.Info($"[{serviceNodeInfo.Name}] サービスノード初期化開始 ({serviceNodeInfo.Priority})");
            _serviceNodeStatusPublisher.Publish(ServiceNodeStatus.StartInitializeServiceNode(serviceNodeInfo));
            // サービス初期化
            LogPublisher.Info($"[{serviceNodeInfo.Name}] サービス初期化 ({serviceNodeInfo.Priority})");
            await StartInitialize(ct);
            // サービスノード初期化完了
            LogPublisher.Info($"[{serviceNodeInfo.Name}] サービスノード初期化完了 ({serviceNodeInfo.Priority})");
            _serviceNodeStatusPublisher.Publish(ServiceNodeStatus.CompleteInitializeServiceNode(serviceNodeInfo));
            // サービスノード開始許可待ち
            LogPublisher.Info($"[{serviceNodeInfo.Name}] サービスノード開始許可待ち");
            await _centralHubStatusSubscriber.FirstAsync(
                ct,
                ev => ev.Phase == CentralHubStatusPhase.AllowStartServiceNode);
            // サービスノード開始
            LogPublisher.Info($"[{serviceNodeInfo.Name}] サービスノード開始");
            _serviceNodeStatusPublisher.Publish(ServiceNodeStatus.StartServiceNode(serviceNodeInfo));
            // サービス開始
            await StartService(ct);
        }

        /// <summary>ノードサービス起動</summary>
        protected abstract UniTask StartInitialize(CancellationToken ct);
        
        /// <summary>ノードサービス開始</summary>
        protected abstract UniTask StartService(CancellationToken ct);

        /// <summary>ログ追加</summary>
        [Obsolete] protected void LogAddLine(string log, string color = null) =>
            LogPublisher.Info($"{log}", string.IsNullOrEmpty(color) ? LogColor : color);

        /// <summary>エラー追加</summary>
        [Obsolete] protected void ErrorAddLine(string log, string color = null) =>
            LogPublisher.Error($"{log}", string.IsNullOrEmpty(color) ? LogColor : color);

        /// <summary>警告追加</summary>
        [Obsolete] protected void WarningAddLine(string log, string color = null) =>
            LogPublisher.Warning($"{log}", string.IsNullOrEmpty(color) ? LogColor : color);

        /// <summary>ログ更新</summary>
        [Obsolete] protected void LogReplaceLine(string log, string color = null) =>
            LogPublisher.RInfo($"{log}", string.IsNullOrEmpty(color) ? LogColor : color);

        /// <summary>リソース開放</summary>
        public abstract void Dispose();
    }
}
