using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using MessagePipe;
using PoppoKoubou.CommonLibrary.AggregateService.Domain;
using PoppoKoubou.CommonLibrary.Log.Domain;
using UnityEngine;
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
        [Inject] public CentralHub(
            IPublisher<LogMessage> logPublisher,
            IPublisher<CentralHubStatus> centralHubStatusPublisher,
            ISubscriber<ServiceNodeStatus> serviceNodeStatusSubscriber)
        {
            Debug.Log($"CentralHub.CentralHub()");
            _logPublisher = logPublisher;
            _centralHubStatusPublisher = centralHubStatusPublisher;
            _serviceNodeStatusSubscriber = serviceNodeStatusSubscriber;
        }

        /// <summary>カラー開始</summary>
        private readonly string _color = "#a0d0ff";

        /// <summary>サービスノードコレクション</summary>
        private Dictionary<int, List<ServiceNodeInfo>> _services = new ();

        /// <summary>サービス集約ハブ初期化</summary>
        public async UniTask StartAsync(CancellationToken ct)
        {
            // 0.1秒待機
            await UniTask.Delay(TimeSpan.FromMilliseconds(100), cancellationToken: ct);

            Debug.Log($"CentralHub.StartAsync()");
            _logPublisher.Publish(LogMessage.AddLine($"サービス集約ハブ起動", LogLevel.Info, _color));

            // 0.1秒待機
            await UniTask.Delay(TimeSpan.FromMilliseconds(100), cancellationToken: ct);

            // サービスノード登録開始
            _centralHubStatusPublisher.Publish(CentralHubStatus.WaitingRegistrationServiceNode());
            _logPublisher.Publish(LogMessage.AddLine($"サービスノード登録開始", LogLevel.Info, _color));
            
            // サービスノード登録待ち
            var dispose1 = _serviceNodeStatusSubscriber.Subscribe(
                ev => {
                    if (!_services.TryGetValue(ev.ServiceNodeInfo.Priority, out var list))
                    {
                        list = new List<ServiceNodeInfo>();
                        _services[ev.ServiceNodeInfo.Priority] = list;
                    }
                    list.Add(ev.ServiceNodeInfo);
                    _logPublisher.Publish(LogMessage.AddLine($"{ev.ServiceNodeInfo.Name} 登録完了 (優先順位: {ev.ServiceNodeInfo.Priority})", LogLevel.Info, _color));
                },
                ev => ev.Phase == ServiceNodeStatusPhase.RegistrationServiceNode
            );
            
            // 3秒待機
            await UniTask.Delay(TimeSpan.FromSeconds(3), cancellationToken: ct);
            
            // サービスノード登録待ち解除
            dispose1.Dispose();
            
            // サービスノードコレクションのキーの順にソートしたリストを作成
            foreach (var priority in _services.Keys.OrderBy(x => x))
            {
                // サービスノード登録開始
                _centralHubStatusPublisher.Publish(CentralHubStatus.WaitingInitializeServiceNode(priority));
                _logPublisher.Publish(LogMessage.AddLine($"サービスノード初期化待ち (優先順位: {priority})", LogLevel.Info, _color));
                
                // サービスノード初期化待ち
                var count = 0;
                var dispose2 = _serviceNodeStatusSubscriber.Subscribe(
                    ev =>
                    {
                        _logPublisher.Publish(LogMessage.AddLine($"{ev.ServiceNodeInfo.Name} 初期化完了 (優先順位: {ev.ServiceNodeInfo.Priority})", LogLevel.Info, _color));
                        count++;
                    },
                    ev => ev.Phase == ServiceNodeStatusPhase.CompleteInitializeServiceNode &&
                          ev.ServiceNodeInfo.Priority == priority
                );
                // count が指定された優先順位のサービスノード数と一致するまで待機
                while (count != _services[priority].Count)
                {
                    await UniTask.Delay(TimeSpan.FromMilliseconds(100), cancellationToken: ct);
                }
                _logPublisher.Publish(LogMessage.AddLine($"サービスノード初期化完了 (優先順位: {priority})", LogLevel.Info, _color));
            }
            
            // サービスノード開始許可
            _centralHubStatusPublisher.Publish(CentralHubStatus.AllowStartServiceNode());
            _logPublisher.Publish(LogMessage.AddLine($"サービスノード開始許可", LogLevel.Info, _color));
        }
    }
}
