using VContainer.Unity;
using Zombies.Bootstrap;
using Zombies.Loading;
using Zombies.UI;

public sealed class MenuController : IStartable, System.IDisposable
{
    private readonly BootCompletion bootCompletion;
    private readonly MenuPage menuPage;
    private readonly SceneLoadingService sceneLoadingService;

    public MenuController(BootCompletion bootCompletion, MenuPage menuPage, SceneLoadingService sceneLoadingService)
    {
        this.bootCompletion = bootCompletion;
        this.menuPage = menuPage;
        this.sceneLoadingService = sceneLoadingService;
    }

    public async void Start()
    {
        await bootCompletion.WaitAsync();
        UnityEngine.Time.timeScale = 1f;
        UnityEngine.Cursor.lockState = UnityEngine.CursorLockMode.Confined;
        UnityEngine.Cursor.visible = true;
        menuPage.PlayRequested += LoadGame;
        menuPage.Show();
    }

    public void Dispose()
    {
        menuPage.PlayRequested -= LoadGame;
        menuPage.Hide();
    }

    private void LoadGame() => sceneLoadingService.Load("Game Scene");
}
