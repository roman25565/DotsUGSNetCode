using UnityEngine;

namespace MainMenu.UI
{
    public class GameStateVisibilityUI : UIPanelBase
    {
        [SerializeField]
        GameState ShowThisWhen;

        void GameStateChanged(GameState state)
        {
            if (!ShowThisWhen.HasFlag(state))
                Hide();
            else
                Show();
        }

        public override void Start()
        {
            base.Start();
            Manager.onGameStateChanged += GameStateChanged;
        }

        void OnDestroy()
        {
            if (Manager == null)
                return;
            Manager.onGameStateChanged -= GameStateChanged;
        }
    }
}