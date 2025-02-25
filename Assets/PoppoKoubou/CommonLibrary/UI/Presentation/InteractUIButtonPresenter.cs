using System;
using UnityEngine;
using UnityEngine.EventSystems;
using MessagePipe;
using PoppoKoubou.CommonLibrary.UI.Domain;
using R3;
using VContainer;

namespace PoppoKoubou.CommonLibrary.UI.Presentation
{
    public class InteractUIButtonPresenter : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler
    {
        [Header("クリック時に発行するメッセージ文字列")]
        [SerializeField] private string message;
        [Header("ドラッグ可能な範囲のRectTransform(1つ上の親以外は多分計算がずれる)")]
        [SerializeField] private RectTransform draggableArea;

        /// <summary>クリック時イベントメッセージ</summary>
        private IPublisher<InteractUI> _interactUIPublisher;
        private RectTransform _rectTransform;
        private readonly Subject<Unit> _pointerUpSubject = new Subject<Unit>();
        private readonly Subject<Vector2> _pointerMoveSubject = new Subject<Vector2>();
        private IDisposable _clickSubscription;
        private IDisposable _dragSubscription;

        private bool _isDragging = false;
        // ドラッグ開始時に、親のローカル座標系におけるポインター位置と子の anchoredPosition の差を保持
        private Vector2 _dragOffset;  

        /// <summary>依存注入</summary>
        [Inject] public void Construct(IPublisher<InteractUI> interactUIPublisher)
        {
            _interactUIPublisher = interactUIPublisher;
        }

        private void Awake()
        {
            _rectTransform = GetComponent<RectTransform>();
            // ドラッグ中の座標更新用に購読を設定
            _dragSubscription = _pointerMoveSubject.Subscribe(HandleDrag);
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            _isDragging = false;
            
            // 親（draggableArea）のローカル座標におけるポインター位置を取得し、
            // その時の子の anchoredPosition との差分をオフセットとして記録
            Vector2 localPoint;
            if (RectTransformUtility.ScreenPointToLocalPointInRectangle(
                    draggableArea, eventData.position, eventData.pressEventCamera, out localPoint))
            {
                _dragOffset = localPoint - _rectTransform.anchoredPosition;
            }
            else
            {
                _dragOffset = Vector2.zero;
            }

            // クリック用タイマーをリセット
            _clickSubscription?.Dispose();

            // 0.3秒待機し、0.3秒以内に離された場合はクリックとして扱う
            _clickSubscription = Observable.Timer(TimeSpan.FromSeconds(0.3))
                .TakeUntil(_pointerUpSubject)
                .Subscribe(
                    _ =>
                    {
                        _isDragging = true; // 0.3秒経過でドラッグモードに移行
                        Debug.Log("Dragging Mode Start");
                    },
                    ex => Debug.LogError("Timer error: " + ex),
                    _ =>
                    {
                        if (!_isDragging)
                        {
                            HandleClick();
                        }
                    }
                );
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            _pointerUpSubject.OnNext(Unit.Default);
        }

        public void OnDrag(PointerEventData eventData)
        {
            if (_isDragging)
            {
                // ドラッグ中は、スクリーン座標を受け取り、購読先で処理
                _pointerMoveSubject.OnNext(eventData.position);
            }
        }

        private void HandleClick()
        {
            if (!_isDragging)
            {
                Debug.Log("Click Detected");
                _interactUIPublisher.Publish(InteractUI.ClickButton(gameObject, message));
            }
        }

        /// <summary>
        /// ドラッグ中のスクリーン座標を親のローカル座標に変換し、
        /// ドラッグ開始時のオフセットを考慮して新しい anchoredPosition を計算・更新します。
        /// </summary>
        private void HandleDrag(Vector2 screenPosition)
        {
            if (!_isDragging) return;

            Vector2 localPoint;
            // Canvas が Screen Space Overlay なら第三引数は null で問題ありません
            if (RectTransformUtility.ScreenPointToLocalPointInRectangle(
                    draggableArea, screenPosition, null, out localPoint))
            {
                // 子の anchoredPosition = (ローカル座標 - オフセット)
                Vector2 targetAnchoredPosition = localPoint - _dragOffset;
                // ClampToDraggableArea で、子の実際の表示位置が親内に収まるよう制限
                targetAnchoredPosition = ClampToDraggableArea(targetAnchoredPosition);
                _rectTransform.anchoredPosition = targetAnchoredPosition;
                Debug.Log($"Dragging to: {targetAnchoredPosition}");
            }
        }

        /// <summary>
        /// 子の anchoredPosition を、親の RectTransform 内に
        /// アイコン全体（Pivot は 0.5,0.5 固定）が収まるようにクランプします。
        /// 
        /// 計算の流れ：
        /// 1. 親の RectTransform.rect から、親内のアンカー基準点（childAnchor）を算出
        /// 2. 子の実際の位置 = (親のアンカー基準点 + anchoredPosition)
        /// 3. 子の実際の位置が、親の境界内（左右上下に子サイズの半分分の余裕）に収まるよう、
        ///    anchoredPosition の最小／最大値を導出しクランプする
        /// </summary>
        private Vector2 ClampToDraggableArea(Vector2 targetAnchoredPosition)
        {
            // 親（draggableArea）のローカル座標系での矩形
            Rect parentRect = draggableArea.rect;
            // 子のアンカー値（anchorMin==anchorMax を前提とする）
            Vector2 childAnchor = _rectTransform.anchorMin;
            // 親内のアンカー基準点は、親の矩形の xMin～xMax, yMin～yMax を補間して求める
            Vector2 anchorPoint = new Vector2(
                Mathf.Lerp(parentRect.xMin, parentRect.xMax, childAnchor.x),
                Mathf.Lerp(parentRect.yMin, parentRect.yMax, childAnchor.y)
            );
            // 子のサイズの半分（例：64x64 なら (32,32)）をマージンとして設定
            Vector2 iconHalfSize = _rectTransform.rect.size * 0.5f;

            // 子の実際の位置 = anchorPoint + anchoredPosition が、
            // 親の矩形内（左：parentRect.xMin, 右：parentRect.xMax, など）に収まる条件：
            // parent's xMin + iconHalfSize.x <= (anchorPoint.x + anchoredPosition.x) <= parent's xMax - iconHalfSize.x
            // すなわち、anchoredPosition.x は：
            // [parentRect.xMin + iconHalfSize.x - anchorPoint.x, parentRect.xMax - iconHalfSize.x - anchorPoint.x]
            float minX = parentRect.xMin + iconHalfSize.x - anchorPoint.x;
            float maxX = parentRect.xMax - iconHalfSize.x - anchorPoint.x;
            float minY = parentRect.yMin + iconHalfSize.y - anchorPoint.y;
            float maxY = parentRect.yMax - iconHalfSize.y - anchorPoint.y;

            float clampedX = Mathf.Clamp(targetAnchoredPosition.x, minX, maxX);
            float clampedY = Mathf.Clamp(targetAnchoredPosition.y, minY, maxY);

            return new Vector2(clampedX, clampedY);
        }

        private void OnDestroy()
        {
            _clickSubscription?.Dispose();
            _dragSubscription?.Dispose();
            _pointerUpSubject.Dispose();
            _pointerMoveSubject.Dispose();
        }
    }
}
