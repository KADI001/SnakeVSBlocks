using System;
using UnityEngine;

namespace SnakeVsBlocks.Model
{
    public class Transformable : IDisposable
    {
        private Vector3 _position;
        private float _rotation;
        private Vector2 _scale;

        public Collider Collider { get; protected set; }

        public event Action Moved;
        public event Action Rotated;
        public event Action Scaled;
        public event Action<Transformable> Destroyed;
        public event Action Collided;
        public event Action Triggered;

        public Transformable(Vector3 position, float rotation, Vector2 scale)
        {
            _position = position;
            _rotation = rotation;
            _scale = scale;
        }

        public Vector2 Forward => Quaternion.Euler(0, 0, Rotation) * Vector2.up;
        public Vector2 Up => Quaternion.Euler(0, 0, Rotation) * Vector2.up;
        public Vector2 Right => Quaternion.Euler(0, 0, Rotation) * Vector2.right;

        public Vector2 Position => _position;
        public float Rotation => _rotation;
        public Vector2 Scale => _scale;

        public void MoveTo(Vector2 position)
        {
            _position = position;

            Moved?.Invoke();
        }

        public void Move(Vector2 delta)
        {
            MoveTo(Position + delta);
        }

        public void Rotate(float delta)
        {
            SetRotation(_rotation + delta);
        }

        public void SetRotation(float rotation)
        {
            _rotation = rotation;

            _rotation = Mathf.Repeat(_rotation, 360);

            Rotated?.Invoke();
        }

        public void SetScale(Vector2 scale)
        {
            _scale = scale;

            Scaled?.Invoke();
        }

        public void LookAt(Vector2 target)
        {
            Rotate(Vector2.SignedAngle(Forward, (target - Position).normalized));
        }

        public void Dispose()
        {
            Destroyed?.Invoke(this);
        }

        public void Collide(Transformable transform, Collision2D collision)
        {
            OnCollided(transform, collision);
            Collided?.Invoke();
        }

        protected virtual void OnCollided(Transformable transform, Collision2D collision)
        {
        }

        public void Trigger(Transformable transform)
        {
            OnTriggered(transform);
            Triggered?.Invoke();
        }

        protected virtual void OnTriggered(Transformable transform)
        {

        }
    }
}