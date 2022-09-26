using UnityEngine;

namespace SnakeVsBlocks.Model
{
    public class Segment : Transformable, IFixedUpdatable, IPausable
    {
        private Transformable _trackedSegment;
        private float _trackingSpeed = 30f;
        private Vector2 _offset = Vector2.down * 0.3f;
        private Vector2 _lastTrackedSegmentPosition;

        private bool _isPaused;

        public bool IsPaused => _isPaused;

        public Segment(Vector2 position, float rotation, Vector2 scale, Transformable trackedSegment) : base(position, rotation, scale) 
        {
            _trackedSegment = trackedSegment;

            _lastTrackedSegmentPosition = GetTargetPosition();
            Collider = new CircleCollider2D(this);
        }

        public Vector2 GetTargetPosition() => _trackedSegment.Position + _offset;
        public CircleCollider2D CircleCollider => base.Collider as CircleCollider2D;

        public void FixedUpdate(float fixedDeltaTime)
        {
            if (IsPaused == true)
                return;

            MoveTo(Vector2.Lerp(Position, _trackedSegment.Position, _trackingSpeed * fixedDeltaTime));

            if (_trackedSegment.Position.y > _lastTrackedSegmentPosition.y)
                _lastTrackedSegmentPosition = _trackedSegment.Position;
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