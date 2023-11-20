using Duende.IdentityServer.Models;

namespace AuthCenterWebApp.Pages.Home.Error;

public class ViewModel
{
    public ViewModel()
    {
    }

    public ViewModel(string error)
    {
        this.Error = new ErrorMessage { Error = error };
    }

    public ErrorMessage? Error { get; set; }
}