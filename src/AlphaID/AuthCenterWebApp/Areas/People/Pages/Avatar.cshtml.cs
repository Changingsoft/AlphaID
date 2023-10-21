using IDSubjects;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AuthCenterWebApp.Areas.People.Pages
{
    public class AvatarModel : PageModel
    {
        NaturalPersonManager personManager;

        public AvatarModel(NaturalPersonManager personManager)
        {
            this.personManager = personManager;
        }

        public async Task<IActionResult> OnGetAsync(string userAnchor)
        {
            var person = await this.personManager.FindByNameAsync(userAnchor) ?? await this.personManager.FindByIdAsync(userAnchor);
            if (person == null)
                return this.NotFound();
            if (person.Avatar != null)
                return this.File(person.Avatar.Data, person.Avatar.MimeType);
            return this.File("~/img/no-picture-avatar.png", "image/png");
        }
    }
}
