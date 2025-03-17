using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using MessagePipe;
using PoppoKoubou.CommonLibrary.AggregateService.Domain;
using PoppoKoubou.CommonLibrary.Log.Domain;
using PoppoKoubou.CommonLibrary.Log.Infrastructure;
using VContainer;
using VContainer.Unity;

namespace PoppoKoubou.CommonLibrary.AggregateService.Application
{
    /// <summary>サービス集約ハブ</summary>
    // ReSharper disable once ClassNeverInstantiated.Global
    public class CentralHub : IAsyncStartable
    {
        /// <summary>ログ送信用Publisher</summary>
        private readonly IPublisher<LogMessage> _logPublisher;
        
        /// <summary>サービス集約ハブステータス送信用Publisher</summary>
        private readonly IPublisher<CentralHubStatus> _centralHubStatusPublisher;

        /// <summary>サービスノードステータス受信用Subscriber</summary>
        private readonly ISubscriber<ServiceNodeStatus> _serviceNodeStatusSubscriber;
        
        /// <summary>依存注入</summary>
        [Inject]
        public CentralHub(
            IPublisher<LogMessage> logPublisher,
            IPublisher<CentralHubStatus> centralHubStatusPublisher,
            ISubscriber<ServiceNodeStatus> serviceNodeStatusSubscriber)
        {
            _logPublisher = logPublisher;
            _centralHubStatusPublisher = centralHubStatusPublisher;
            _serviceNodeStatusSubscriber = serviceNodeStatusSubscriber;
            _logPublisher.Debug("CentralHub.CentralHub()");
        }

        /// <summary>ログ用カラー</summary>
        private readonly string _color = "#ffff00";

        public async UniTask StartAsync(CancellationToken ct)
        {
            // 初期ウェイト
            await UniTask.Delay(TimeSpan.FromMilliseconds(100), cancellationToken: ct);
            _logPublisher.Debug("CentralHub.StartAsync()");
            _logPublisher.Debug("サービス集約ハブ起動", _color);
            await UniTask.Delay(TimeSpan.FromMilliseconds(100), cancellationToken: ct);

            // 登録バッチ用のサービスリスト（ループ全体で保持し、後で Clear する）
            var batchServices = new Dictionary<int, List<ServiceNodeInfo>>();

            // 永続的な登録イベントの購読（Dispose しない）
            var permanentRegistrationDisposable = _serviceNodeStatusSubscriber.Subscribe(
                ev =>
                {
                    if (ev.Phase == ServiceNodeStatusPhase.RegistrationServiceNode)
                    {
                        if (!batchServices.TryGetValue(ev.ServiceNodeInfo.Priority, out var innerList))
                        {
                            innerList = new List<ServiceNodeInfo>();
                            batchServices[ev.ServiceNodeInfo.Priority] = innerList;
                        }
                        // 重複チェック（永続購読側）
                        if (innerList.Contains(ev.ServiceNodeInfo))
                        {
                            _logPublisher.Info($"{ev.ServiceNodeInfo.Name} 登録済み（重複）(優先順位: {ev.ServiceNodeInfo.Priority})", _color);
                        }
                        else
                        {
                            innerList.Add(ev.ServiceNodeInfo);
                            _logPublisher.Info($"{ev.ServiceNodeInfo.Name} 継続登録完了 (優先順位: {ev.ServiceNodeInfo.Priority})", _color);
                        }
                    }
                },
                ev => ev.Phase == ServiceNodeStatusPhase.RegistrationServiceNode
            );

            // 無限ループでバッチ処理を実行
            while (!ct.IsCancellationRequested)
            {
                // サービス登録開始フェイズ
                _centralHubStatusPublisher.Publish(CentralHubStatus.WaitingRegistrationServiceNode());
                _logPublisher.Info("サービスノード登録開始", _color);

                // 最初の１件目の登録を待機
                var firstEvent = await _serviceNodeStatusSubscriber.FirstAsync(
                    ct,
                    ev => ev.Phase == ServiceNodeStatusPhase.RegistrationServiceNode);

                // 重複チェック（初回側）
                if (!batchServices.TryGetValue(firstEvent.ServiceNodeInfo.Priority, out var list))
                {
                    list = new List<ServiceNodeInfo>();
                    batchServices[firstEvent.ServiceNodeInfo.Priority] = list;
                }
                if (list.Contains(firstEvent.ServiceNodeInfo))
                {
                    _logPublisher.Info($"{firstEvent.ServiceNodeInfo.Name} 初回登録（重複）(優先順位: {firstEvent.ServiceNodeInfo.Priority})", _color);
                }
                else
                {
                    list.Add(firstEvent.ServiceNodeInfo);
                    _logPublisher.Info($"{firstEvent.ServiceNodeInfo.Name} 初回登録完了 (優先順位: {firstEvent.ServiceNodeInfo.Priority})", _color);
                }

                // 登録受付期間（例：1.5秒）を待機
                await UniTask.Delay(TimeSpan.FromSeconds(1.5), cancellationToken: ct);

                // 登録フェイズ終了：現在のバッチ内容をディープコピーして初期化用リストとする
                var initBatchServices = new Dictionary<int, List<ServiceNodeInfo>>();
                foreach (var kv in batchServices)
                {
                    initBatchServices[kv.Key] = new List<ServiceNodeInfo>(kv.Value);
                }
                // 登録バッチはクリアして、次のバッチ受付に備える
                batchServices.Clear();

                // サービス初期化フェイズ
                foreach (var priority in initBatchServices.Keys.OrderBy(x => x))
                {
                    var count = 0;
                    // 初期化完了イベントの購読（対象は、initBatchServices に含まれるサービスのみ）
                    var initDisposable = _serviceNodeStatusSubscriber.Subscribe(
                        ev =>
                        {
                            // もし対象リストに含まれていればカウント
                            if (!initBatchServices[priority].Contains(ev.ServiceNodeInfo)) return;
                            count++;
                            _logPublisher.Info(
                                $"{ev.ServiceNodeInfo.Name} 初期化完了 (優先順位: {ev.ServiceNodeInfo.Priority}) {count} / {initBatchServices[priority].Count}",
                                _color);
                        },
                        ev => ev.Phase == ServiceNodeStatusPhase.CompleteInitializeServiceNode &&
                              ev.ServiceNodeInfo.Priority == priority &&
                              initBatchServices[priority].Contains(ev.ServiceNodeInfo)
                    );

                    _logPublisher.Info($"サービスノード初期化待ち (優先順位: {priority}) {initBatchServices[priority].Count} 件", _color);
                    _centralHubStatusPublisher.Publish(CentralHubStatus.WaitingInitializeServiceNode(priority));

                    // すべてのサービスの初期化完了を待機
                    while (count < initBatchServices[priority].Count)
                    {
                        await UniTask.Delay(TimeSpan.FromMilliseconds(1000), cancellationToken: ct);
                        _logPublisher.Debug($"Check {count} / {initBatchServices[priority].Count}", "#ff3030");
                    }
                    initDisposable.Dispose();
                    _logPublisher.Info($"サービスノード初期化完了 (優先順位: {priority})", _color);
                }

                // サービス開始許可フェイズ
                _centralHubStatusPublisher.Publish(CentralHubStatus.AllowStartServiceNode());
                _logPublisher.Info("サービスノード開始許可", _color);

                // ※次のバッチ開始までのウェイトが必要なら、ここで待機（例：短いDelayなど）
            }
            // 永続的な登録購読は Dispose しません
        }
    }
}
