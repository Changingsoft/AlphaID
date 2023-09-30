using Microsoft.AspNetCore.Mvc;

namespace AdminWebApp.Components;

public class VideoPlayer : ViewComponent
{
    public IViewComponentResult Invoke()
    {
        return this.Content(string.Empty);
    }
}
