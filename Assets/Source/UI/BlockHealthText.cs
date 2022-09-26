using UnityEngine;
using TMPro;
using SnakeVsBlocks.Model;

namespace SnakeVsBlocks.UI
{
    [RequireComponent(typeof(TMP_Text))]
    public class BlockHealthText : MonoBehaviour
    {
        private Block _block;
        private TMP_Text _textMesh;

        private void OnEnable()
        {
            _block.HealthChanged += OnHealthChanged;
        }

        private void OnDisable()
        {
            _block.HealthChanged -= OnHealthChanged;
        }

        public void Init(Block block)
        {
            _block = block;
            _textMesh = GetComponent<TMP_Text>();

            UpdateText();

            enabled = true;
        }

        private void OnHealthChanged()
        {
            UpdateText();
        }

        private void UpdateText()
        {
            _textMesh.text = _block.Health.ToString();
        }
    }
}
