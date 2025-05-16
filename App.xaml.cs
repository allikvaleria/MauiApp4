namespace MauiApp4
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();
            MainPage = new Startpage();
        }

        protected override Window CreateWindow(IActivationState? activationState)
        {
            return new Window(new AppShell());
        }
    }
}