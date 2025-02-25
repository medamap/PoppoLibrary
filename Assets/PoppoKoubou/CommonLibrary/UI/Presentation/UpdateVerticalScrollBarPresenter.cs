using System;
using MessagePipe;
using PoppoKoubou.CommonLibrary.UI.Domain;
using UnityEngine;
using UnityEngine.UI;
using VContainer;

namespace PoppoKoubou.CommonLibrary.UI.Presentation
{
    [RequireComponent(typeof(ScrollRect))]
    public class UpdateVerticalScrollBarPresenter : MonoBehaviour
    {
        private ScrollRect _scrollRect;

        // 自動スクロール判定：下端からのオフセットがこの値以下なら下端とみなす
        [SerializeField] private float autoScrollPixelThreshold = 10f;

        // 微小な変化なら更新しないための許容差（normalized 値）
        [SerializeField] private float normalizedTolerance = 0.001f;

        private ISubscriber<UpdateUI> _updateUISubscriber;
        private IDisposable _subscription;

        // ユーザー操作時に記録する「上からのピクセルオフセット」
        private float _lastOffsetFromTop = 0f;
        // 同時に記録する「下からのピクセルオフセット」
        private float _lastOffsetFromBottom = 0f;

        // プログラム的更新中の onValueChanged イベント抑制用フラグ
        private bool _suppressOnValueChangedUpdate = false;

        [Inject]
        public void Construct(ISubscriber<UpdateUI> updateUISubscriber)
        {
            _updateUISubscriber = updateUISubscriber;
        }

        private void Awake()
        {
            _scrollRect = GetComponent<ScrollRect>();
        }

        private void Start()
        {
            // Content の Pivot を (0.5, 0) に強制設定（これにより下端が基準になります）
            if (_scrollRect.content != null)
            {
                Vector2 desiredPivot = new Vector2(0.5f, 0f);
                if (_scrollRect.content.pivot != desiredPivot)
                {
                    _scrollRect.content.pivot = desiredPivot;
                    Canvas.ForceUpdateCanvases();
                    //Debug.Log("[AutoScroll] Content pivot updated to (0.5, 0).");
                }
            }
            // 初期状態は下端に固定
            _scrollRect.verticalNormalizedPosition = 0f;
            Canvas.ForceUpdateCanvases();

            // 初期状態の場合、下端なら上からのオフセットは新しい maxScrollable に等しく、下からは 0
            _lastOffsetFromTop = 0f;
            _lastOffsetFromBottom = 0f;

            // ユーザー操作検知
            _scrollRect.onValueChanged.AddListener(OnScrollValueChanged);

            _subscription = _updateUISubscriber.Subscribe(OnUpdateUI, ev => ev.Type == UpdateUIType.UpdateVerticalSize);
        }

        private void OnDestroy()
        {
            _subscription?.Dispose();
            _scrollRect.onValueChanged.RemoveListener(OnScrollValueChanged);
        }

        // ユーザー操作時に呼ばれる：上から・下からのオフセットを更新する
        private void OnScrollValueChanged(Vector2 normalizedPos)
        {
            if (_suppressOnValueChangedUpdate)
                return;

            Canvas.ForceUpdateCanvases();
            RectTransform viewport = _scrollRect.viewport != null ? _scrollRect.viewport : (RectTransform)_scrollRect.transform;
            float contentHeight = _scrollRect.content.rect.height;
            float viewportHeight = viewport.rect.height;
            float maxScrollable = contentHeight - viewportHeight;

            if (maxScrollable <= 0)
            {
                _lastOffsetFromTop = 0f;
                _lastOffsetFromBottom = 0f;
            }
            else
            {
                // verticalNormalizedPosition: 1 = 上端、0 = 下端
                // 上からのオフセット（ピクセル）＝ (1 - normalizedPos.y) * maxScrollable
                _lastOffsetFromTop = (1 - normalizedPos.y) * maxScrollable;
                // 下からのオフセット（ピクセル）＝ normalizedPos.y * maxScrollable
                _lastOffsetFromBottom = normalizedPos.y * maxScrollable;
            }
            //Debug.Log($"[AutoScroll] OnScrollValueChanged: lastOffsetFromTop = {lastOffsetFromTop}, lastOffsetFromBottom = {lastOffsetFromBottom}");
        }

        // UpdateUI イベント受信時：新しい Content サイズに合わせてスクロール位置を更新
        private void OnUpdateUI(UpdateUI updateEvent)
        {
            Canvas.ForceUpdateCanvases();

            // 必要に応じて Content の Pivot を再設定
            if (_scrollRect.content != null)
            {
                Vector2 desiredPivot = new Vector2(0.5f, 0f);
                if (_scrollRect.content.pivot != desiredPivot)
                {
                    _scrollRect.content.pivot = desiredPivot;
                    Canvas.ForceUpdateCanvases();
                    //Debug.Log("[AutoScroll] Content pivot re-updated to (0.5, 0).");
                }
            }

            RectTransform viewport = _scrollRect.viewport != null ? _scrollRect.viewport : (RectTransform)_scrollRect.transform;
            float contentHeight = _scrollRect.content.rect.height;
            float viewportHeight = viewport.rect.height;
            float newMaxScrollable = contentHeight - viewportHeight;
            if(newMaxScrollable <= 0)
                return;

            // 新しい下からのオフセットを計算：更新前に記録した lastOffsetFromBottom を用いる
            // ここで、もし lastOffsetFromBottom が autoScrollPixelThreshold 以下なら、
            // ユーザーは下端に近いと判断して、強制スクロールを実施
            if (_lastOffsetFromBottom <= autoScrollPixelThreshold)
            {
                _suppressOnValueChangedUpdate = true;
                _scrollRect.verticalNormalizedPosition = 0f;
                _lastOffsetFromTop = newMaxScrollable; // 下端の場合、上からのオフセットは newMaxScrollable
                _lastOffsetFromBottom = 0f;
                //Debug.Log("[AutoScroll] Auto-scroll triggered: setting position to bottom.");
                _suppressOnValueChangedUpdate = false;
                return;
            }
            else
            {
                // ユーザーが下端にいない場合は、上からのオフセットを維持するように更新
                float targetNormalized = 1 - (_lastOffsetFromTop / newMaxScrollable);
                float currentNormalized = _scrollRect.verticalNormalizedPosition;
                //Debug.Log($"[AutoScroll] OnUpdateUI: contentHeight: {contentHeight}, viewportHeight: {viewportHeight}, newMaxScrollable: {newMaxScrollable}, currentNormalized: {currentNormalized}, targetNormalized: {targetNormalized}");
                _suppressOnValueChangedUpdate = true;
                if (Mathf.Abs(currentNormalized - targetNormalized) > normalizedTolerance)
                {
                    _scrollRect.verticalNormalizedPosition = targetNormalized;
                    //Debug.Log($"[AutoScroll] Adjusting scroll: setting verticalNormalizedPosition to {targetNormalized} (maintaining top offset {lastOffsetFromTop}).");
                }
                else
                {
                    //Debug.Log($"[AutoScroll] No adjustment needed: currentNormalized ({currentNormalized}) is close to targetNormalized ({targetNormalized}).");
                }
                _suppressOnValueChangedUpdate = false;
            }
        }
    }
}
