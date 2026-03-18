using UnityEngine;
using System.Linq;
using MantenseiLib;

namespace GameJam_URA.UI
{
    public class UIViewHub : SingletonMonoBehaviour<UIViewHub>
    {
        [GetComponents(HierarchyRelation.Children, true)]
        IUIView[] Views { get; set; }

        T Get<T>() where T : class, IUIView => Views.OfType<T>().First();

        public GameHUDView GameHUD => Get<GameHUDView>();
        public MenuView Menu => Get<MenuView>();
        public CommentView Comment => Get<CommentView>();
        public RegisterView Register => Get<RegisterView>();
    }
}
