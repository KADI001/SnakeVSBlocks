using System;
using System.Collections.Generic;
using UnityEngine;
using SnakeVsBlocks.Model;

namespace SnakeVsBlocks
{
    public class SnakeCollisionHandler : IFixedUpdatable, IDisposable
    {
        public const float MinCheckDistance = 3f;

        private List<Model.Collider> _obstacles;
        private Snake _snake;
        private float _minX, _maxX;
        private LevelGenerator _levelGenerator;

        public SnakeCollisionHandler(Snake snake, LevelGenerator levelGenerator, float minX, float maxX)
        {
            _obstacles = new List<Model.Collider>();
            _snake = snake;
            _levelGenerator = levelGenerator;
            _minX = minX;
            _maxX = maxX;

            OnEnable();
        }

        public void FixedUpdate(float fixedDeltaTime)
        {
            Vector2 delta1 = _snake.VerticalVelocity * fixedDeltaTime;
            Vector2 delta2 = _snake.HorizontalVelocity * fixedDeltaTime;

            ClampMove(delta1 + delta2);

            for (int i = 0; i < _obstacles.Count; i++)
            {
                if (_obstacles[i] is Model.BoxCollider)
                {
                    Vector2 temp = _obstacles[i].Transform.Position - _snake.Position;

                    if (temp.sqrMagnitude <= MinCheckDistance)
                    {
                        if (Model.Physics.TryHandleCollision(_snake.CircleCollider, _obstacles[i] as Model.BoxCollider, out Model.Collision2D collision))
                        {
                            _snake.Collide(_obstacles[i].Transform, collision);
                        }
                    }
                }
                else if (_obstacles[i] is Model.CircleCollider2D)
                {
                    if (_snake.CircleCollider.Collided(_obstacles[i] as Model.CircleCollider2D) != null)
                    {
                        _obstacles[i].Transform.Trigger(_snake);
                    }
                }
            }

            _snake.VerticalVelocity = Vector2.zero;
            _snake.HorizontalVelocity = Vector2.zero;
        }

        private void ClampMove(Vector2 delta)
        {
            Vector2 newPosition = _snake.Position + delta;
            newPosition.x = Mathf.Clamp(newPosition.x, _minX, _maxX);

            _snake.MoveTo(newPosition);
        }

        private Transformable GetClosestObject(Transformable[] transforms)
        {
            float minDistance = float.PositiveInfinity;
            Transformable closestObject = null;

            for (int i = 0; i < transforms.Length; i++)
            {
                float newDistance = (_snake.Position - transforms[i].Position).sqrMagnitude;

                if (newDistance < minDistance)
                {
                    minDistance = newDistance;
                    closestObject = transforms[i];
                }
            }

            return closestObject;
        }

        private void OnEnable()
        {
            foreach (var obstacle in _obstacles)
            {
                obstacle.Transform.Destroyed += OnObstacleDestroyed;
            }

            _levelGenerator.Generated += OnObjectGenerated;
        }

        private void OnDisable()
        {
            foreach (var obstacle in _obstacles)
            {
                obstacle.Transform.Destroyed -= OnObstacleDestroyed;
            }

            _levelGenerator.Generated -= OnObjectGenerated;
        }

        private void OnObjectGenerated(IEnumerable<Transformable> objects)
        {
            foreach (var obj in objects)
            {
                _obstacles.Add(obj.Collider);
                obj.Destroyed += OnObstacleDestroyed;
            }
        }

        private void OnObstacleDestroyed(Transformable obstacle)
        {
            _obstacles.Remove(obstacle.Collider);
        }

        public void Dispose()
        {
            OnDisable();
        }
    }
}
