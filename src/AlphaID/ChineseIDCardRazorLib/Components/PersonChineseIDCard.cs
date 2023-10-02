using Microsoft.AspNetCore.Mvc;

namespace ChineseIDCardRazorLib.Components;
internal class PersonChineseIDCard : ViewComponent
{
    public IViewComponentResult Invoke()
    {
        return View();
    }
}
