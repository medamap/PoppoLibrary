using UnityEngine;
using UnityEngine.EventSystems;
using MessagePipe;
using PoppoKoubou.CommonLibrary.UI.Domain;
using VContainer;

namespace PoppoKoubou.CommonLibrary.UI.Presentation
{
    public class UIInteractMouseClickDetector : MonoBehaviour, IPointerClickHandler
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
    }
}