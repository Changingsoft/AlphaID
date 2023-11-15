using IdSubjects;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AuthCenterWebApp.Areas.People.Pages
{
    public class AvatarModel : PageModel
    {
        private readonly NaturalPersonManager personManager;

        public AvatarModel(NaturalPersonManager personManager)
        {
            this.personManager = personManager;
        }

        public async Task<IActionResult> OnGetAsync(string anchor)
        {
            var person = await this.personManager.FindByNameAsync(anchor) ?? await this.personManager.FindByIdAsync(anchor);
            if (person == null)
                return this.NotFound();
            if (person.ProfilePicture != null)
                return this.File(person.ProfilePicture.Data, person.ProfilePicture.MimeType);
            return this.File("~/img/no-picture-avatar.png", "image/png");
        }
    }
}
