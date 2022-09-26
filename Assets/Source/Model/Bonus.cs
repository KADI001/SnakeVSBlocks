using UnityEngine;

namespace SnakeVsBlocks.Model
{
    public class Bonus : Transformable 
    {
        public readonly int Value;
        public const int MinValue = 1;

        public Bonus(Vector2 position, float rotation, Vector2 scale, int value) : base(position, rotation, scale)
        {
            Value = value < MinValue ? MinValue : value;

            Collider = new CircleCollider2D(this);
        }

        public CircleCollider2D CircleCollider => base.Collider as CircleCollider2D;

        public void Apply(Snake snake)
        {
            for (int i = 0; i < Value; i++)
            {
                snake.IncreaseLength();
            }
        }

        protected override void OnTriggered(Transformable transform)
        {
            if (transform is Snake)
            {
                Apply((Snake)transform);
            }

            Dispose();
        }
    }
}