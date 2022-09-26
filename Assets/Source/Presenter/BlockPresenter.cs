using SnakeVsBlocks.Model;
using SnakeVsBlocks.UI;
using UnityEngine;

namespace SnakeVsBlocks.Presenters
{
    public class BlockPresenter : Presenter
    {
        [SerializeField] private BlockHealthText _healthText;
        public new Block Model => base.Model as Block;

        protected override void OnInitializing()
        {
            _healthText.Init(Model);
        }
    }
}
