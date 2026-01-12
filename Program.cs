using FirstDesktopApp.UI;

namespace FirstDesktopApp
{
    internal static class Program
    {
        [STAThread]
        static void Main()
        {
            ApplicationConfiguration.Initialize();
            Application.Run(new GameContext());
        }
    }

    public class GameContext : ApplicationContext
    {
        public GameContext()
        {
            var mainMenu = new MainMenuForm();
            mainMenu.Show();
        }
    }
}
