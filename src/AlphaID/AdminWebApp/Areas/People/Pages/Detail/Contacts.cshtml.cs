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

            //��֤�ƶ��绰���������ʼ���ַ�Ƿ��ѱ�ע�᣿
            if (!MobilePhoneNumber.TryParse(this.Input.PhoneNumber, out var phoneNumber))
            {
                this.ModelState.AddModelError("", "�ƶ��绰�����ʽ����ȷ��");
                return this.Page();
            }

            if (manager.Users.Any(p => p.Id != anchor && p.PhoneNumber == phoneNumber.ToString()))
            {
                this.ModelState.AddModelError("", "�ƶ��绰�����ѱ�ע�ᡣ");
            }
            if (manager.Users.Any(p => p.Id != anchor && p.Email == this.Input.Email))
            {
                this.ModelState.AddModelError("", "�����ʼ���ַ�ѱ�ע�ᡣ");
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
