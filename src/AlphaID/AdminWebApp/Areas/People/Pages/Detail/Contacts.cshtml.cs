using IdSubjects;
using IdSubjects.Subjects;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace AdminWebApp.Areas.People.Pages.Detail
{
    public class ContactsModel : PageModel
    {
        private readonly NaturalPersonManager manager;

        public ContactsModel(NaturalPersonManager manager)
        {
            this.manager = manager;
        }

        public NaturalPerson Data { get; set; } = default!;

        [BindProperty]
        public InputModel Input { get; set; } = default!;

        public string? OperationMessage { get; set; }

        public async Task<IActionResult> OnGet(string anchor)
        {
            var data = await this.manager.FindByIdAsync(anchor);
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
            var data = await this.manager.FindByIdAsync(anchor);
            if (data == null)
                return this.NotFound();

            this.Data = data;

            //��֤�ƶ��绰���������ʼ���ַ�Ƿ��ѱ�ע�᣿
            if (!MobilePhoneNumber.TryParse(this.Input.PhoneNumber, out var phoneNumber))
            {
                this.ModelState.AddModelError("", "�ƶ��绰�����ʽ����ȷ��");
                return this.Page();
            }

            if (this.manager.Users.Any(p => p.Id != anchor && p.PhoneNumber == phoneNumber.ToString()))
            {
                this.ModelState.AddModelError("", "�ƶ��绰�����ѱ�ע�ᡣ");
            }
            if (this.manager.Users.Any(p => p.Id != anchor && p.Email == this.Input.Email))
            {
                this.ModelState.AddModelError("", "�����ʼ���ַ�ѱ�ע�ᡣ");
            }

            if (!this.ModelState.IsValid)
            {
                return this.Page();
            }

            var result = await this.manager.UpdateAsync(this.Data);
            if (result.Succeeded)
            {
                this.OperationMessage = "�޸ĳɹ���";
                return this.Page();
            }

            foreach (var error in result.Errors)
            {
                this.ModelState.AddModelError("", error.Description);
            }
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
