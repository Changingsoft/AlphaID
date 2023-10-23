using IDSubjects;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AuthCenterWebApp.Areas.Profile.Pages
{
    public class AvatarModel : PageModel
    {
        private readonly NaturalPersonManager personManager;

        public AvatarModel(NaturalPersonManager personManager)
        {
            this.personManager = personManager;
        }

        public async Task<IActionResult> OnGetAsync()
        {
            var person = await this.personManager.GetUserAsync(User);
            if (person == null)
                return this.NotFound();

            if (person.Avatar != null)
                return this.File(person.Avatar.Data, person.Avatar.MimeType);
            return this.File("~/img/no-picture-avatar.png", "image/png");
        }
    }
}
