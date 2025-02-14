using MessagePipe;
using PoppoKoubou.CommonLibrary.UI.Domain;
using UniRx;
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
        [Inject] private IPublisher<InteractUI> _interactUIPublisher;
        
        /// <summary>コンポーネント初期化</summary>
        private void Start()
        {
            // ボタンコンポーネント取得
            var button = GetComponent<Button>();
            if (button == null)
            {
                Debug.LogError($"Button component not found.");
                return;
            }
            // ボタンクリック処理
            button.OnClickAsObservable()
                .Subscribe(_ => _interactUIPublisher.Publish(new InteractUI(InteractUIType.ClickButton, gameObject, message)))
                .AddTo(this);
        }
    }
}
