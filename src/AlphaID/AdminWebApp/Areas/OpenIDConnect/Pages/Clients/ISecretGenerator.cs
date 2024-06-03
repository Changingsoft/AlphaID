namespace AdminWebApp.Areas.OpenIDConnect.Pages.Clients;

public interface ISecretGenerator
{
    string Generate();
}