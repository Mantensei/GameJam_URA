using MantenseiLib;
using UnityEngine;
using UnityEngine.InputSystem;

namespace GameJam_URA
{
    public class CustomerHoverManager
    {
        public static CustomerHoverManager Instance { get; private set; }

        readonly float hoverRadius;

        public CustomerMineAI HoveredCustomer { get; private set; }

        public CustomerHoverManager(GameObject observerHost, float hoverRadius = 1f)
        {
            this.hoverRadius = hoverRadius;
            Instance = this;

            new LifecycleObserverFactory(observerHost)
                .OnUpdated(Tick)
                .OnDestroyed(Stop);
        }

        void Stop()
        {
            if (Instance == this) Instance = null;
        }

        void Tick()
        {
            var pointer = Pointer.current;
            if (pointer == null)
            {
                SetHovered(null);
                return;
            }

            var cam = Camera.main;
            var worldPos = (Vector2)cam.ScreenToWorldPoint(pointer.position.ReadValue());

            CustomerMineAI closest = null;
            float closestDist = hoverRadius;

            foreach (var ai in CustomerMineAI.Instances)
            {
                var dist = Vector2.Distance(worldPos, (Vector2)ai.transform.position);
                if (dist < closestDist)
                {
                    closestDist = dist;
                    closest = ai;
                }
            }

            SetHovered(closest);
        }

        void SetHovered(CustomerMineAI ai)
        {
            if (HoveredCustomer == ai) return;
            HoveredCustomer = ai;
        }
    }
}
