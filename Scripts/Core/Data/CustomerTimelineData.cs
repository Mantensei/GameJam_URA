using System;
using UnityEngine;

namespace GameJam_URA
{
    [Serializable]
    public class ActionTimeline
    {

    }

    [CreateAssetMenu(fileName = "NewActionTimelineData", menuName = "GameJam/ActionTimelineData")]
    public class ActionTimelineData : ScriptableObject
    {
        [SerializeField] int i;
        [SerializeField] ActionTimeline[] timelines;

        public ActionTimeline[] Timelines => timelines;
    }
}
