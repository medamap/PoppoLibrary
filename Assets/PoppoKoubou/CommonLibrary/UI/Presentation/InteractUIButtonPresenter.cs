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
        [SerializeField] private string message;
        [SerializeField] private RectTransform draggableArea; // ドラッグ可能エリア（Canvas の RectTransform）

        private IPublisher<InteractUI> _interactUIPublisher;
        private RectTransform _rectTransform;

        private Subject<Unit> _pointerUpSubject = new Subject<Unit>();
        private Subject<Vector2> _pointerMoveSubject = new Subject<Vector2>();
        private IDisposable _clickSubscription;
        private IDisposable _dragSubscription;

        private bool _isDragging = false;
        private Vector2 _startPosition;
        private Vector2 _dragOffset;  // ボタンとポインター間のオフセット

        [Inject]
        public void Construct(IPublisher<InteractUI> interactUIPublisher)
        {
            _interactUIPublisher = interactUIPublisher;
        }

        private void Awake()
        {
            _rectTransform = GetComponent<RectTransform>();

            // ドラッグ中の座標更新用に購読を追加
            _dragSubscription = _pointerMoveSubject.Subscribe(HandleDrag);
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            _isDragging = false; // ドラッグ状態をリセット
            _startPosition = _rectTransform.anchoredPosition;

            // ドラッグ開始時に、ポインターとボタンの現在位置の差を記録
            Vector2 pointerLocalPos;
            if (RectTransformUtility.ScreenPointToLocalPointInRectangle(
                    draggableArea, eventData.position, eventData.pressEventCamera, out pointerLocalPos))
            {
                _dragOffset = pointerLocalPos - _rectTransform.anchoredPosition;
            }
            else
            {
                _dragOffset = Vector2.zero;
            }

            // クリックストリームをリセット
            _clickSubscription?.Dispose();

            _clickSubscription = Observable.Timer(TimeSpan.FromSeconds(0.3))
                .TakeUntil(_pointerUpSubject)
                .Subscribe(
                    _ =>
                    {
                        _isDragging = true; // 0.3秒経過でドラッグモードへ移行
                        Debug.Log("Dragging Mode Start");
                    },
                    ex => Debug.LogError("Timer error: " + ex),
                    _ =>
                    {
                        // 0.3秒以内に離された場合はクリックとして処理
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
                Debug.Log($"Dragging: {eventData.position}");
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

        private void HandleDrag(Vector2 screenPosition)
        {
            if (!_isDragging) return;

            Vector2 localPoint;
            // eventData.pressEventCamera の代わりに null ではなく、カメラを渡すとより正確になる場合があります
            if (RectTransformUtility.ScreenPointToLocalPointInRectangle(
                    draggableArea, screenPosition, null, out localPoint))
            {
                // ドラッグ開始時のオフセットを考慮して新しい位置を計算
                Vector2 targetPosition = localPoint - _dragOffset;
                targetPosition = ClampToDraggableArea(targetPosition);
                _rectTransform.anchoredPosition = targetPosition;
                Debug.Log($"Dragging to: {targetPosition}");
            }
        }

        private Vector2 ClampToDraggableArea(Vector2 position)
        {
            Vector2 minBounds = draggableArea.rect.min;
            Vector2 maxBounds = draggableArea.rect.max;

            return new Vector2(
                Mathf.Clamp(position.x, minBounds.x, maxBounds.x),
                Mathf.Clamp(position.y, minBounds.y, maxBounds.y)
            );
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
