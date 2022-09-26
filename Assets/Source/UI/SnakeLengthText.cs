using SnakeVsBlocks.Model;
using TMPro;
using UnityEngine;

namespace SnakeVsBlocks.UI
{
    [RequireComponent(typeof(RectTransform))]
    public class SnakeLengthText : MonoBehaviour
    {
        [SerializeField] private TMP_Text _textMesh;
        private RectTransform _rectTransform;
        private Snake _snake;

        private void LateUpdate()
        {
            transform.position = _snake.Position;
        }

        public void Init(Snake snake)
        {
            _snake = snake;
            _rectTransform = GetComponent<RectTransform>();

            UpdateText();

            enabled = true;
        }

        private void OnEnable()
        {
            _snake.LengthChanged += OnSnakeLengthChanged;
        }

        private void OnDisable()
        {
            _snake.LengthChanged -= OnSnakeLengthChanged;
        }

        private void OnSnakeLengthChanged()
        {
            UpdateText();
        }

        private void UpdateText()
        {
            _textMesh.text = _snake.Length.ToString();
        }
    }
}
