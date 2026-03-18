using MantenseiLib;
using UnityEngine;

namespace GameJam_URA
{
    public class URA_PlayerReferenceHub : MonoBehaviour
    {
        [GetComponent]
        public PlayerReferenceHub PlayerHub { get; private set; }

        [GetComponent(HierarchyRelation.Children)]
        public Interactor Interactor { get; private set; }

        [GetComponent(HierarchyRelation.Children)]
        public InteractableDetector Detector { get; private set; }

        [GetComponent(HierarchyRelation.Children)]
        public Sitter Sitter { get; private set; }
    }
}
