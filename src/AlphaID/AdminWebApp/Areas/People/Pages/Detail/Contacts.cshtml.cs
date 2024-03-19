using IdSubjects;
using IdSubjects.Subjects;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace AdminWebApp.Areas.People.Pages.Detail
{
    public class ContactsModel(NaturalPersonManager manager) : PageModel
    {
        public NaturalPerson Data { get; set; } = default!;

        [BindProperty]
        public InputModel Input { get; set; } = default!;

        public IdentityResult? Result { get; set; }

        public async Task<IActionResult> OnGetAsync(string anchor)
        {
            var data = await manager.FindByIdAsync(anchor);
            if (data == null)
                return this.NotFound();

            this.Data = data;
            this.Input = new InputModel
            {
                PhoneNumber = this.Data.PhoneNumber,
                Email = this.Data.Email,
            };
            return this.Page();
        }

        public async Task<IActionResult> OnPostAsync(string anchor)
        {
            var data = await manager.FindByIdAsync(anchor);
            if (data == null)
                return this.NotFound();

            this.Data = data;

            //验证移动电话号码或电子邮件地址是否已被注册？
            if (!MobilePhoneNumber.TryParse(this.Input.PhoneNumber, out var phoneNumber))
            {
                this.ModelState.AddModelError("", "移动电话号码格式不正确。");
                return this.Page();
            }

            if (manager.Users.Any(p => p.Id != anchor && p.PhoneNumber == phoneNumber.ToString()))
            {
                this.ModelState.AddModelError("", "移动电话号码已被注册。");
            }
            if (manager.Users.Any(p => p.Id != anchor && p.Email == this.Input.Email))
            {
                this.ModelState.AddModelError("", "电子邮件地址已被注册。");
            }

            if (!this.ModelState.IsValid)
            {
                return this.Page();
            }

            this.Result = await manager.UpdateAsync(this.Data);
            return this.Page();
        }

        public class InputModel
        {
            [Display(Name = "Phone number")]
            [StringLength(15, ErrorMessage = "Validate_StringLength")]
            [DataType(DataType.PhoneNumber)]
            public string? PhoneNumber { get; set; }

            [Display(Name = "Email")]
            [StringLength(100, ErrorMessage = "Validate_StringLength")]
            [EmailAddress]
            public string? Email { get; set; }
        }
    }
}
