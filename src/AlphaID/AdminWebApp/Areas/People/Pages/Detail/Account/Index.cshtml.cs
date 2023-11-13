using IDSubjects;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace AdminWebApp.Areas.People.Pages.Detail.Account
{
    public class IndexModel : PageModel
    {
        private readonly NaturalPersonManager userManager;

        public IndexModel(NaturalPersonManager userManager)
        {
            this.userManager = userManager;
        }

        public NaturalPerson Data { get; set; } = default!;

        public bool HasPassword { get; set; }

        [BindProperty]
        public InputModel Input { get; set; } = default!;

        public string? OperationResultMessage { get; set; }

        public async Task<IActionResult> OnGet(string anchor)
        {
            var person = await this.userManager.FindByIdAsync(anchor);
            if (person == null)
                return this.NotFound();

            this.Data = person;
            this.HasPassword = await this.userManager.HasPasswordAsync(this.Data);
            this.Input = new()
            {
                UserName = this.Data.UserName
            };

            return this.Page();
        }

        public async Task<IActionResult> OnPostAsync(string anchor)
        {
            var person = await this.userManager.FindByIdAsync(anchor);
            if (person == null)
                return this.NotFound();

            this.Data = person;
            this.HasPassword = await this.userManager.HasPasswordAsync(this.Data);

            if (!this.ModelState.IsValid)
            {
                return this.Page();
            }

            if (this.userManager.Users.Any(p => p.Id != this.Data.Id && p.UserName == this.Input.UserName))
            {
                this.ModelState.AddModelError("", "不能与其他账户名相同");
                return this.Page();
            }

            await this.userManager.SetUserNameAsync(this.Data, this.Input.UserName);
            this.OperationResultMessage = "操作已成功。";
            return this.Page();
        }

        public class InputModel
        {
            [Display(Name = "User name")]
            [StringLength(50, ErrorMessage = "Validate_StringLength")]
            public string UserName { get; set; } = default!;
        }

    }
}
