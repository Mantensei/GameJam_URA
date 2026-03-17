using System.Collections.Generic;
using UnityEngine;

namespace GameJam_URA.Prototype
{
    public class InteractableDetector : MonoBehaviour
    {
        List<IInteractable> nearbyTargets = new List<IInteractable>();

        public IInteractable CurrentTarget
        {
            get
            {
                for (int i = nearbyTargets.Count - 1; i >= 0; i--)
                {
                    if (nearbyTargets[i] as MonoBehaviour == null)
                    {
                        nearbyTargets.RemoveAt(i);
                        continue;
                    }
                }
                if (nearbyTargets.Count == 0) return null;

                IInteractable closest = null;
                float closestDist = float.MaxValue;
                Vector2 pos = transform.position;

                foreach (var target in nearbyTargets)
                {
                    float dist = Vector2.Distance(pos, ((MonoBehaviour)target).transform.position);
                    if (dist < closestDist)
                    {
                        closestDist = dist;
                        closest = target;
                    }
                }
                return closest;
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
