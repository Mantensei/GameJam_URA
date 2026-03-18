using System;
using System.Collections.Generic;
using MantenseiLib;
using UnityEngine;

namespace GameJam_URA
{
    public class InteractableDetector : MonoBehaviour
    {
        List<IInteractable> nearbyTargets = new List<IInteractable>();
        public event Action<IInteractable> OnTargetChanged;

        public IInteractable CurrentTarget { get; private set; }

        void Update()
        {
            IInteractable closest = null;
            float closestDist = float.MaxValue;

            for (int i = nearbyTargets.Count - 1; i >= 0; i--)
            {
                if (!nearbyTargets[i].IsSafe())
                {
                    nearbyTargets.RemoveAt(i);
                    continue;
                }

                float dist = Vector2.Distance(transform.position, nearbyTargets[i].transform.position);
                if (dist < closestDist)
                {
                    closestDist = dist;
                    closest = nearbyTargets[i];
                }
            }

            if (closest != CurrentTarget)
            {
                CurrentTarget = closest;
                OnTargetChanged?.Invoke(CurrentTarget);
            }
        }

        void OnTriggerEnter2D(Collider2D other)
        {
            var interactable = other.GetComponent<IInteractable>();
            if (interactable != null && !nearbyTargets.Contains(interactable))
            {
                nearbyTargets.Add(interactable);
            }
        }

        void OnTriggerExit2D(Collider2D other)
        {
            var exitObj = other.gameObject;
            for (int i = nearbyTargets.Count - 1; i >= 0; i--)
            {
                if (nearbyTargets[i].gameObject == exitObj)
                {
                    nearbyTargets.RemoveAt(i);
                    break;
                }
            }
        }
    }
}
