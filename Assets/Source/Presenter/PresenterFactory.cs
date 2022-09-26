using UnityEngine;
using SnakeVsBlocks.Model;

namespace SnakeVsBlocks.Presenters
{
    public class PresenterFactory : MonoBehaviour
    {
        [SerializeField] private SegmentPresenter _segmentPrefab;
        [SerializeField] private BlockPresenter _blockPrefab;
        [SerializeField] private BorderPresenter _borderPrefab;
        [SerializeField] private BonusPresenter _bonusPrefab;

        public SegmentPresenter CreateSegment(Segment segment)
        {
            return (SegmentPresenter)Create(segment, _segmentPrefab);
        }

        public BlockPresenter CreateBlock(Block block)
        {
            return (BlockPresenter)Create(block, _blockPrefab);
        }

        public BonusPresenter CreateBonus(Bonus bonus)
        {
            return (BonusPresenter)Create(bonus, _bonusPrefab);
        }

        public BorderPresenter CreateBorder(Border border)
        {
            return (BorderPresenter)Create(border, _borderPrefab);
        }

        private Presenter Create(Transformable model, Presenter prefab)
        {
            Presenter presenter = Instantiate(prefab);
            presenter.Init(model);
            return presenter;
        }
    }
}
