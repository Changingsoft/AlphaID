using System.ComponentModel.DataAnnotations;
using IdSubjects;
using IdSubjects.Subjects;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace AdminWebApp.Areas.People.Pages.Detail;

public class ContactsModel(NaturalPersonManager manager) : PageModel
{
    public NaturalPerson Data { get; set; } = default!;

    [BindProperty]
    public InputModel Input { get; set; } = default!;

    public IdentityResult? Result { get; set; }

    public async Task<IActionResult> OnGetAsync(string anchor)
    {
        NaturalPerson? data = await manager.FindByIdAsync(anchor);
        if (data == null)
            return NotFound();

        Data = data;
        Input = new InputModel
        {
            PhoneNumber = Data.PhoneNumber,
            Email = Data.Email
        };
        return Page();
    }

    public async Task<IActionResult> OnPostAsync(string anchor)
    {
        NaturalPerson? data = await manager.FindByIdAsync(anchor);
        if (data == null)
            return NotFound();

        Data = data;

        //��֤�ƶ��绰���������ʼ���ַ�Ƿ��ѱ�ע�᣿
        if (!MobilePhoneNumber.TryParse(Input.PhoneNumber, out MobilePhoneNumber phoneNumber))
        {
            ModelState.AddModelError("", "�ƶ��绰�����ʽ����ȷ��");
            return Page();
        }

        if (manager.Users.Any(p => p.Id != anchor && p.PhoneNumber == phoneNumber.ToString()))
            ModelState.AddModelError("", "�ƶ��绰�����ѱ�ע�ᡣ");
        if (manager.Users.Any(p => p.Id != anchor && p.Email == Input.Email))
            ModelState.AddModelError("", "�����ʼ���ַ�ѱ�ע�ᡣ");

        if (!ModelState.IsValid) return Page();

        Result = await manager.UpdateAsync(Data);
        return Page();
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