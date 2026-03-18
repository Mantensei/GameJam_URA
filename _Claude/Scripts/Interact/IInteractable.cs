namespace GameJam_URA
{
    public interface IInteractable : IMonoBehaviour
    {
        void Interact(URA_PlayerReferenceHub player);
        string ActiontLabel { get; }
    }
}
