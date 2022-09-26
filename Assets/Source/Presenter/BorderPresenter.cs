using SnakeVsBlocks.Model;

namespace SnakeVsBlocks.Presenters
{
    public class BorderPresenter : Presenter
    {
        public new Border Model => base.Model as Border;
    }
}
