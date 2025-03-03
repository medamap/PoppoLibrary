using UnityEngine;
using UnityEngine.EventSystems;
using MessagePipe;
using PoppoKoubou.CommonLibrary.UI.Domain;
using VContainer;

#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

namespace PoppoKoubou.CommonLibrary.UI.Presentation
{
    public class UIInteractUniversalClickDetector : MonoBehaviour, IPointerClickHandler
    {
        private IPublisher<ClickUI> _clickUIPublisher;

        [Header("クリック時に発行するメッセージ")]
        [SerializeField] private string customMessage = "";

        [Inject] public void Construct(IPublisher<ClickUI> clickUIPublisher)
        {
            _clickUIPublisher = clickUIPublisher;
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            Vector2 localPoint;
            if (RectTransformUtility.ScreenPointToLocalPointInRectangle(
                GetComponent<RectTransform>(), eventData.position, eventData.pressEventCamera, out localPoint))
            {
                _clickUIPublisher.Publish(new ClickUI(gameObject, localPoint, customMessage));
            }
        }

        private void Update()
        {
            Vector2 touchPosition = Vector2.zero;
            bool touchDetected = false;

#if ENABLE_INPUT_SYSTEM
            if (Touchscreen.current?.primaryTouch.press.isPressed == true)
            {
                touchPosition = Touchscreen.current.primaryTouch.position.ReadValue();
                touchDetected = true;
            }
#else
            if (Input.touchCount > 0)
            {
                Touch touch = Input.GetTouch(0);
                touchPosition = touch.position;
                touchDetected = true;
            }
#endif

            if (touchDetected)
            {
                Vector2 localPoint;
                if (RectTransformUtility.ScreenPointToLocalPointInRectangle(
                    GetComponent<RectTransform>(), touchPosition, null, out localPoint))
                {
                    _clickUIPublisher.Publish(new ClickUI(gameObject, localPoint, customMessage));
                }
            }
        }
    }
}
