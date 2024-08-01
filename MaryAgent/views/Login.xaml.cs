namespace MaryAgent.views;

public partial class Login : ContentPage
{
	public Login()
	{
		InitializeComponent();
	}

    private async void Button_Clicked(object sender, EventArgs e)
    {
        try
        {
            //WebAuthenticatorResult authResult = await WebAuthenticator.Default.AuthenticateAsync(
            //    new Uri("http://namipc.com:5000/login"),
            //    new Uri("myapp://"));

            //string accessToken = authResult?.AccessToken;

            //go to the chat page using shell navigation
            Application.Current.MainPage = new AppShell();

            

            // Do something with the token
        }
        catch (TaskCanceledException ex)
        {
            // show alert
            await DisplayAlert("Authentication Error", ex.Message, "OK");
        }

    }

    private async void Google_Button_Clicked(object sender, EventArgs e)
    {
        try
        {
            //WebAuthenticatorResult authResult = await WebAuthenticator.Default.AuthenticateAsync(
            //    new Uri("http://namipc.com:5000/login"),
            //    new Uri("myapp://"));

            //string accessToken = authResult?.AccessToken;

            //go to the chat page using shell navigation
            Application.Current.MainPage = new AppShell();



            // Do something with the token
        }
        catch (TaskCanceledException ex)
        {
            // show alert
            await DisplayAlert("Authentication Error", ex.Message, "OK");
        }

    }
}