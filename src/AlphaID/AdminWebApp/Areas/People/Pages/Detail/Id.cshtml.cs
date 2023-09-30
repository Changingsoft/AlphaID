using IDSubjects;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace AdminWebApp.Areas.People.Pages.Detail
{
    public class IdModel : PageModel
    {
        private readonly NaturalPersonManager userManager;

        public IdModel(NaturalPersonManager userManager)
        {
            this.userManager = userManager;
        }

        public NaturalPerson Data { get; set; } = default!;

        public bool HasPassword { get; set; }

        [BindProperty]
        public InputModel Input { get; set; } = default!;

        public string? OperationResultMessage { get; set; }

        public async Task<IActionResult> OnGet(string id)
        {
            var person = await this.userManager.FindByIdAsync(id);
            if (person == null)
                return this.NotFound();

            this.Data = person;
            this.HasPassword = await this.userManager.HasPasswordAsync(this.Data);
            this.Input = new()
            {
                UserName = this.Data.UserName,
            };

            return this.Page();
        }

        public async Task<IActionResult> OnPostAsync(string id)
        {
            var person = await this.userManager.FindByIdAsync(id);
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
                this.ModelState.AddModelError("", "�����������˻�����ͬ");
                return this.Page();
            }

            this.Data.UserName = this.Input.UserName;
            await this.userManager.UpdateAsync(this.Data);
            this.OperationResultMessage = "�����ѳɹ���";
            return this.Page();
        }

        public class InputModel
        {
            [Display(Name = "�˻���")]
            [StringLength(50)]
            public string UserName { get; set; } = default!;
        }
    }
}
