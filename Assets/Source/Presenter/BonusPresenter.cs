using SnakeVsBlocks.Model;
using SnakeVsBlocks.UI;
using UnityEngine;

namespace SnakeVsBlocks.Presenters
{
    public class BonusPresenter : Presenter
    {
        [SerializeField] private BonusValueText _valueText;
        public new Bonus Model => base.Model as Bonus;

        protected override void OnInitializing()
        {
            _valueText.Init(Model);
        }
    }
}
