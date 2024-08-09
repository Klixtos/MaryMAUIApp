using MaryAgent.views;

namespace MaryAgent
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();
            RegisterRoutes();
        }

        private void RegisterRoutes()
        {
            Routing.RegisterRoute(nameof(MaryChat), typeof(MaryChat));
            Routing.RegisterRoute(nameof(Account), typeof(Account));
            Routing.RegisterRoute(nameof(AssesmentsPage), typeof(AssesmentsPage));
            Routing.RegisterRoute(nameof(AssesmentFilesPage), typeof(AssesmentFilesPage));
        }

        private void LogoutMenuItem_Clicked(object sender, EventArgs e)
        {
            //TODO: clear everithing and go to login page
            Application.Current.MainPage = new Login();
        }
    }
}
