using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace SnakeVsBlocks.Model
{
    public class Snake : Transformable, IFixedUpdatable, IPausable
    {
        private List<Segment> _segments;
        private float _verticalSpeed = 3f;
        private float _horizontalSpeed = 10f;
        private float _bounceDistance = 0.3f;
        private Vector2 _segmentSize => Scale * 0.85f;
        private bool _isPaused;

        public int Length => _segments.Count;
        public Segment LastSegment => _segments?.Last();
        public CircleCollider2D CircleCollider => base.Collider as CircleCollider2D;
        public bool IsPaused => _isPaused;

        public Vector2 VerticalVelocity;
        public Vector2 HorizontalVelocity;

        public event Action LengthChanged;
        public event Action LengthIncreased;
        public event Action LengthReduced;

        public Snake(Vector2 position, float rotation, Vector2 scale) : base(position, rotation, scale) 
        {
            _segments = new List<Segment>();
            Collider = new CircleCollider2D(this);
        }

        public void FixedUpdate(float fixedDeltaTime)
        {
            if (Length <= 0 || IsPaused == true)
                return;

            VerticalVelocity = Vector2.up * _verticalSpeed;
            HorizontalVelocity = Vector2.right * (Forward * _horizontalSpeed).x;  
        }

        public void Bounce(Vector2 direction)
        {
            direction = direction.normalized;

            Move(direction * _bounceDistance);
        }

        protected override void OnCollided(Transformable transform, Collision2D collision)
        {
            if (transform is Block block && collision.Normal.y == 1)
            {
                Bounce(Vector2.down);
                TryReduceLength(block);
            } 
        }

        public void IncreaseLength()
        {
            Segment newSegment;
            Vector2 position;

            if (_segments.Count > 0)
            {
                position = LastSegment.Position;
                newSegment = new Segment(position, LastSegment.Rotation, _segmentSize, LastSegment);
            }
            else
            {
                position = Position - (Vector2.up * (Scale.y * 0.5f + _segmentSize.y * 0.5f));
                newSegment = new Segment(position, Rotation, _segmentSize, this);
            }

            _segments.Add(newSegment);

            LengthChanged?.Invoke();
            LengthIncreased?.Invoke();
        }

        public bool TryReduceLength(Block block)
        {
            if (Length <= 0)
                return false;

            Segment lastSegment = LastSegment;

            _segments.Remove(lastSegment);
            lastSegment.Dispose();

            block.TryReduceHealth();

            LengthChanged?.Invoke();
            LengthReduced?.Invoke();

            return true;
        }

        public void Pause()
        {
            _isPaused = true;
        }

        public void Resume()
        {
            _isPaused = false;
        }
    }
}