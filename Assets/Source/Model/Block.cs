using System;
using UnityEngine;

namespace SnakeVsBlocks.Model
{
    public class Block : Transformable
    {
        private int _health;
        private const int _minHealth = 1;

        public event Action HealthChanged;

        public Block(Vector2 position, float rotation, Vector2 scale, int health) : base(position, rotation, scale)
        {
            _health = health < _minHealth ? _minHealth : health;
            Collider = new BoxCollider(this);
        }

        public int Health => _health;
        public BoxCollider BoxCollider => base.Collider as BoxCollider;

        public void TryReduceHealth()
        {
            _health--;

            HealthChanged?.Invoke();

            if (_health <= 0)
                Dispose();
        }
    }
}