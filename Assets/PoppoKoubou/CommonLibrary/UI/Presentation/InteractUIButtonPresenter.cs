using System;
using Cysharp.Threading.Tasks;
using Cysharp.Threading.Tasks.Linq;
using MessagePipe;
using PoppoKoubou.CommonLibrary.UI.Domain;
using UnityEngine;
using UnityEngine.UI;
using VContainer;

namespace PoppoKoubou.CommonLibrary.UI.Presentation
{
    public class InteractUIButtonPresenter : MonoBehaviour
    {
        /// <summary>UIメッセージ</summary>
        [SerializeField] private string message;
        /// <summary>UIインタラクトメッセージ送信用パブリッシャー</summary>
        private IPublisher<InteractUI> _interactUIPublisher;
        
        /// <summary>依存注入</summary>
        [Inject] public void Construct(
            IPublisher<InteractUI> interactUIPublisher)
        {
            _interactUIPublisher = interactUIPublisher;
        }
        
        /// <summary>コンポーネント初期化</summary>
        private async void Start()
        {
            try
            {
                // ボタンコンポーネント取得
                if (!TryGetComponent<Button>(out var button))
                {
                    Debug.LogError($"Button component not found.");
                    return;
                }
                await button.OnClickAsAsyncEnumerable(this.destroyCancellationToken).ForEachAwaitWithCancellationAsync(
                    async (_, ct) => _interactUIPublisher.Publish(InteractUI.ClickButton(gameObject, message)),
                    destroyCancellationToken);
            }
            catch (Exception e)
            {
                Debug.LogError(e);
            }
        }
    }
}
