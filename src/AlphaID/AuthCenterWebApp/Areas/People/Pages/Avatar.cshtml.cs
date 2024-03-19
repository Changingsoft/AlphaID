using IdSubjects;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AuthCenterWebApp.Areas.People.Pages
{
    public class AvatarModel(NaturalPersonManager personManager) : PageModel
    {
        public async Task<IActionResult> OnGetAsync(string anchor)
        {
            var person = await personManager.FindByNameAsync(anchor) ?? await personManager.FindByIdAsync(anchor);
            if (person == null)
                return this.NotFound();
            if (person.ProfilePicture != null)
                return this.File(person.ProfilePicture.Data, person.ProfilePicture.MimeType);
            return this.File("~/img/no-picture-avatar.png", "image/png");
        }
    }
}
