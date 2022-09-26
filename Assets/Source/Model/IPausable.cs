namespace SnakeVsBlocks.Model
{
    public interface IPausable
    {
        public bool IsPaused { get; }

        public void Pause();
        public void Resume();
    }
}