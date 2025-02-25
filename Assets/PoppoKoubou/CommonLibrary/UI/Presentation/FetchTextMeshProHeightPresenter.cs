using MessagePipe;
using PoppoKoubou.CommonLibrary.UI.Domain;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using VContainer;

namespace PoppoKoubou.CommonLibrary.UI.Presentation
{
    public class FetchTextMeshProHeightPresenter : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI targetText;
        private IPublisher<UpdateUI> _updateUIPublisher;
        private float _lastHeight = 0f;
        private const float Threshold = 1f; // 高さの変化がこれ以上であればイベント発行

        [Inject]
        public void Construct(IPublisher<UpdateUI> updateUIPublisher)
        {
            _updateUIPublisher = updateUIPublisher;
        }

        private void Start()
        {
            if (targetText == null)
            {
                targetText = GetComponent<TextMeshProUGUI>();
            }
            // 初期状態の高さを計測
            MeasureAndUpdate();
        }

        private void LateUpdate()
        {
            MeasureAndUpdate();
        }

        private void MeasureAndUpdate()
        {
            // 最新のメッシュとレイアウトを更新
            targetText.ForceMeshUpdate();
            LayoutRebuilder.ForceRebuildLayoutImmediate(targetText.rectTransform);

            // 現在の高さを取得（preferredHeight を使用）
            float currentHeight = targetText.preferredHeight;

            // 前回との差が Threshold を超えている場合、イベント発行
            if (Mathf.Abs(currentHeight - _lastHeight) > Threshold)
            {
                _lastHeight = currentHeight;
                // UpdateUI イベントを作成して発行（float の値で送信）
                _updateUIPublisher.Publish(UpdateUI.UpdateVerticalSize(currentHeight));
            }
        }
    }
}
