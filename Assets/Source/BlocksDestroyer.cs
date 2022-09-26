using System;
using System.Collections.Generic;
using UnityEngine;
using SnakeVsBlocks.Model;

namespace SnakeVsBlocks
{
    public class BlocksDestroyer : IUpdatable, IDisposable
    {
        private List<Transformable> _obstacles;
        private LevelGenerator _levelGenerator;
        private Camera _camera;

        public Vector3 DeleteZone => _camera.transform.position - Vector3.up * 6;

        public BlocksDestroyer(Camera camera, LevelGenerator levelGenerator)
        {
            _obstacles = new List<Transformable>();
            _camera = camera;
            _levelGenerator = levelGenerator;

            OnEnable();
        }

        private void OnEnable()
        {
            _levelGenerator.Generated += OnObjectsGenerated;
        }

        private void OnDisable()
        {
            _levelGenerator.Generated -= OnObjectsGenerated;
        }

        private void OnObjectsGenerated(IEnumerable<Transformable> objects)
        {
            foreach (var obj in objects)
            {
                _obstacles.Add(obj);
            }
        }

        public void Update(float deltaTime)
        {
            for (int i = 0; i < _obstacles.Count; i++)
            {
                if (IsOutOfDeleteZone(_obstacles[i]))
                {
                    _obstacles[i].Dispose();
                }
            }
        }

        private bool IsOutOfDeleteZone(Transformable transform)
        {
            return transform.Position.y < DeleteZone.y;
        }

        public void Dispose()
        {
            OnDisable();
        }
    }
}
