using SnakeVsBlocks.Model;
using SnakeVsBlocks.UI;
using System.Collections.Generic;
using UnityEngine;

namespace SnakeVsBlocks.Presenters
{
    public class SnakePresenter : Presenter
    {
        [SerializeField] private PresenterFactory _factory;
        [SerializeField] private SnakeLengthText _lengthText;
        private List<SegmentPresenter> _segmentPresenters;

        public new Snake Model => base.Model as Snake;

        protected override void OnInitializing()
        {
            _segmentPresenters = new List<SegmentPresenter>();

            _lengthText.Init(Model);
        }

        protected override void OnEnabling()
        {
            Model.LengthIncreased += OnLengthIncreased;
        }

        protected override void OnDisabling()
        {
            Model.LengthIncreased -= OnLengthIncreased;
        }

        private void OnLengthIncreased()
        {
            var newSegment = _factory.CreateSegment(Model.LastSegment);
            
            _segmentPresenters.Add(newSegment);
        }
    }
}
