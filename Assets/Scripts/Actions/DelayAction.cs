using System;
using UnityEngine;

namespace Actions
{
    public class DelayAction : Action
    {
        [SerializeField]
        private float duration;

        private float _timer;

        private ActionFinishCallback _callback;
        
        public override void PerformAction(ActionFinishCallback callback)
        {
            _timer = duration;
            _callback = callback;
        }

        private void Update()
        {
            switch (_timer)
            {
                case > 0:
                    _timer -= Time.deltaTime;
                    break;
                case > -100f:
                    _callback();
                    _timer = -200f;
                    break;
            }
        }
    }
}