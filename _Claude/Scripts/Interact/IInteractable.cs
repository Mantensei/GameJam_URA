namespace GameJam_URA.Prototype
{
    public interface IInteractable : IMonoBehaviour
    {
        void Interact(RestaurantInputHandler player);
        bool CanInteract(RestaurantInputHandler player);
        string InteractLabel { get; }
    }
}
