using System;
using MessagePipe;
using PoppoKoubou.CommonLibrary.UI.Domain;
using UnityEngine;
using UnityEngine.Serialization;
using VContainer;

namespace PoppoKoubou.CommonLibrary.UI.Presentation
{
    public class ToggleActivePresenter : MonoBehaviour
    {
        /// <summary>UI操作購読</summary>
        private ISubscriber<InteractUI> _interactUISubscriber;
        private IDisposable _disposable;

        /// <summary>イベントのトリガー文字列を指定する </summary>
        [Header("イベントのトリガー文字列を指定する")]
        [SerializeField] private string triggerString = "ToggleLogWindow";
        
        /// <summary>アクティブ状態を切り替えるゲームオブジェクト</summary>
        [FormerlySerializedAs("_targetGameObject")]
        [Header("アクティブ状態を切り替えるゲームオブジェクト")]
        [SerializeField] private GameObject targetGameObject;
        
        /// <summary>初期アクティブ状態</summary>
        [FormerlySerializedAs("_isInitialActive")]
        [Header("初期アクティブ状態")]
        [SerializeField] private bool isInitialActive = false;
        
        /// <summary>依存注入</summary>
        [Inject] public void Construct(ISubscriber<InteractUI> interactUISubscriber)
        {
            _interactUISubscriber = interactUISubscriber;
        }

        /// <summary>コンポーネント初期化</summary>
        private async void Start()
        {
            // 初期アクティブ状態設定
            targetGameObject.SetActive(isInitialActive);
            // ログウインドウトグルメッセージ受信
            var disposableBag = DisposableBag.CreateBuilder();
            _interactUISubscriber.Subscribe(
                    // イベント受信でアクティブ状態を切り替える
                message =>
                    {
                        isInitialActive = !isInitialActive;
                        targetGameObject.SetActive(isInitialActive);
                        Debug.Log($"_isInitialActive = {isInitialActive}");
                        Debug.Log($"_targetGameObject.SetActive({isInitialActive})");
                    }, message => string.Equals(message.Message, triggerString))
                .AddTo(disposableBag);
            _disposable = disposableBag.Build();
        }
        
        /// <summary>リソース開放</summary>
        private void OnDestroy()
        {
            _disposable?.Dispose();
        }
    }
}