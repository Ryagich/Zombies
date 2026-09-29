namespace Zombies.UI
{
    public abstract class BasePage
    {
        public abstract PageType Type { get; }
        public abstract void Show();
        public abstract void Hide();
    }
}
