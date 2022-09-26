using SnakeVsBlocks.Model;
using TMPro;
using UnityEngine;

namespace SnakeVsBlocks.UI
{
    [RequireComponent(typeof(TMP_Text))]
    public class BonusValueText : MonoBehaviour
    {
        private Bonus _bonus;
        private TMP_Text _textMesh;

        internal void Init(Bonus bonus)
        {
            _textMesh = GetComponent<TMP_Text>();
            _bonus = bonus;

            UpdateDisplayingText();
        }

        private void UpdateDisplayingText()
        {
            _textMesh.text = _bonus.Value.ToString();
        }
    }
}
