using SnakeVsBlocks.Model;

namespace SnakeVsBlocks.Presenters
{
    public class SegmentPresenter : Presenter
    {
        public new Segment Model => base.Model as Segment;
    }
}
