using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace PoppoKoubou.CommonLibrary.Log.Presentation
{
    /// <summary>
    /// TextMeshProUGUI のサイズを親（Canvas）の横幅にフィットさせ、縦サイズはテキスト量に応じて拡張する
    /// - 横幅：親（Canvas）と同じサイズ
    /// - 縦幅：最小値は Canvas の高さ、それ以上はテキストの長さに応じて拡張
    /// </summary>
    [RequireComponent(typeof(RectTransform), typeof(TextMeshProUGUI))]
    public class FitToParentSizePresenter : MonoBehaviour
    {
        [SerializeField] RectTransform _parentRect;
        [SerializeField] float _minHeight = 0;
        private RectTransform _rectTransform;
        private TextMeshProUGUI _tmpText;
        //private float _minHeight;

        private void Awake()
        {
            _rectTransform = GetComponent<RectTransform>();
            _tmpText = GetComponent<TextMeshProUGUI>();
        }

        private void Start()
        {
            _minHeight = _parentRect.rect.height;
            FitToParent();
        }

        private void Update()
        {
            FitToParent();
        }

        /// <summary>
        /// 親の幅にフィットし、縦のサイズをテキスト量に応じて拡張
        /// </summary>
        private void FitToParent()
        {
            if (_rectTransform.parent is RectTransform parentRect)
            {
                // 横幅を親の幅にフィット
                _rectTransform.sizeDelta = new Vector2(parentRect.rect.width, _rectTransform.sizeDelta.y);
            }

            // 縦幅をテキストの内容に合わせる（最小は _minHeight ）
            _tmpText.ForceMeshUpdate(); // 追加（テキスト更新の強制）
            float preferredHeight = Mathf.Max(_tmpText.preferredHeight, _minHeight);
            
            if (Mathf.Abs(preferredHeight - _rectTransform.sizeDelta.y) > 0.1f)
            {
                _rectTransform.sizeDelta = new Vector2(_rectTransform.sizeDelta.x, preferredHeight);
            }
        }
    }
}