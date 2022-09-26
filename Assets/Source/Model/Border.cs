using UnityEngine;

namespace SnakeVsBlocks.Model
{
    public class Border : Transformable
    {
        public Border(Vector2 position, float rotation, Vector2 scale) : base(position, rotation, scale)
        {
            Collider = new BoxCollider(this);
        }

        public BoxCollider BoxCollider => base.Collider as BoxCollider;
    }
}