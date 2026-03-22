using MantenseiLib;
using UnityEngine;

namespace GameJam_URA
{
    public class SpriteOutlineHighlight : MonoBehaviour
    {
        URA_PlayerReferenceHub _playerHub;
        SpriteRenderer spriteRenderer;
        Color originalColor;
        CustomerMineAI ai;
        bool hovered;

        void Awake()
        {
            _playerHub = GetComponentInParent<URA_PlayerReferenceHub>();
            spriteRenderer = _playerHub.PlayerHub.AnimationManagerReference.GetComponent<SpriteRenderer>();
            originalColor = spriteRenderer.color;
            ai = GetComponentInChildren<CustomerMineAI>();
        }

        void Update()
        {
            var manager = CustomerHoverManager.Instance;
            if (manager == null) return;

            bool nowHovered = manager.HoveredCustomer == ai;
            if (nowHovered == hovered) return;
            hovered = nowHovered;
            spriteRenderer.color = hovered ? Color.yellow : originalColor;
        }
    }
}
