using System;
using UnityEngine;
using SnakeVsBlocks.Model;

namespace SnakeVsBlocks.Presenters
{
    public class Presenter : MonoBehaviour
    {
        public Transformable Model;
        private IUpdatable _updatable;
        private IFixedUpdatable _fixedUpdatable;

        public event Action Inited;

        private void Update()
        {
            
            _updatable?.Update(Time.deltaTime);
        }

        private void FixedUpdate()
        {
            _fixedUpdatable?.FixedUpdate(Time.fixedDeltaTime);
        }

        private void OnEnable()
        {
            OnEnabling();

            Model.Moved += OnMoved;
            Model.Rotated += OnRotated;
            Model.Scaled += OnScaled;
            Model.Destroyed += OnDestroyed;
        }

        private void OnDisable()
        {
            OnDisabling();

            Model.Moved -= OnMoved;
            Model.Rotated -= OnRotated;
            Model.Scaled -= OnScaled;
            Model.Destroyed -= OnDestroyed;
        }

        protected virtual void OnEnabling() { }
        protected virtual void OnDisabling() { }

        public void Init(Transformable transformable)
        {

            Model = transformable;

            _updatable = transformable is IUpdatable ? (IUpdatable)transformable : null;
            _fixedUpdatable = transformable is IFixedUpdatable ? (IFixedUpdatable)transformable : null;

            OnInitializing();

            OnMoved();
            OnRotated();
            OnScaled();

            enabled = true;

            Inited?.Invoke();
        }

        protected virtual void OnInitializing() { }

        private void OnScaled()
        {
            transform.localScale = Model.Scale;
        }

        private void OnMoved()
        {
            transform.position = Model.Position;
        }

        private void OnRotated()
        {
            transform.eulerAngles = Vector3.forward * Model.Rotation;
        }

        private void OnDestroyed(Transformable transform)
        {
            Destroy(this.gameObject);
        }
    }
}
