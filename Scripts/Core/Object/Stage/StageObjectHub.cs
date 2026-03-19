using System.Collections.Generic;
using System.Linq;
using MantenseiLib;
using UnityEngine;

namespace GameJam_URA
{
    public class StageObjectHub : SingletonMonoBehaviour<StageObjectHub>
    {
        [GetComponents(HierarchyRelation.Children, true)]
        IInteractable[] _interactables;
        public IInteractable[] Interactables => _interactables;

        public IReadOnlyList<Seat> Seats { get; private set; }
        public IReadOnlyList<Register> Registers { get; private set; }
        public IReadOnlyList<Exit> Exits { get; private set; }
        public IReadOnlyList<Toilet> Toilets { get; private set; }

        void Start()
        {
            CacheByType();
        }

        void CacheByType()
        {
            Seats = _interactables.OfType<Seat>().ToArray();
            Registers = _interactables.OfType<Register>().ToArray();
            Exits = _interactables.OfType<Exit>().ToArray();
            Toilets = _interactables.OfType<Toilet>().ToArray();
        }
    }
}
