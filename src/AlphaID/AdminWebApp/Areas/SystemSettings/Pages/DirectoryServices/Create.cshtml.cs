using DirectoryLogon;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace AdminWebApp.Areas.SystemSettings.Pages.DirectoryServices;

public class CreateModel : PageModel
{
    private readonly DirectoryServiceManager directoryServiceManager;

    public CreateModel(DirectoryServiceManager directoryServiceManager)
    {
        this.directoryServiceManager = directoryServiceManager;
    }

    [BindProperty]
    public InputModel Input { get; set; } = default!;

    public void OnGet()
    {
    }

    public async Task<IActionResult> OnPostAsync()
    {
        var directoryService = new DirectoryService()
        {
            Name = this.Input.Name,
            ServerAddress = this.Input.ServerAddress,
            RootDN = this.Input.RootDN,
            DefaultUserAccountOU = this.Input.DefaultUserOU,
            UpnSuffix = this.Input.UpnSuffix,
            UserName = this.Input.UserName,
            Password = this.Input.Password,
        };

        var result = await this.directoryServiceManager.CreateAsync(directoryService);
        if (!result.IsSuccess)
        {
            foreach (var error in result.Errors)
            {
                this.ModelState.AddModelError("", error);
            }
            return this.Page();
        }
        return this.RedirectToPage("Index");
    }

    public class InputModel
    {
        [Display(Name = "名称")]
        [StringLength(50)]
        public string Name { get; set; } = default!;

        [Display(Name = "服务地址", Prompt = "ldap.example.com")]
        [StringLength(50)]
        public string ServerAddress { get; set; } = default!;

        [Display(Name = "用户名（可选）")]
        [StringLength(50)]
        [DataType(DataType.Password)]
        public string? UserName { get; set; }

        [Display(Name = "密码（可选）")]
        [StringLength(50)]
        [DataType(DataType.Password)]
        public string? Password { get; set; }

        [Display(Name = "根DN", Prompt = "DC=example,DC=com")]
        [StringLength(150)]
        public string RootDN { get; set; } = default!;

        [Display(Name = "User OU", Prompt = "OU=Users,DC=example,DC=com")]
        [StringLength(150)]
        public string DefaultUserOU { get; set; } = default!;

        [Display(Name = "UPN后缀", Prompt = "example.com")]
        [StringLength(20)]
        public string UpnSuffix { get; set; } = default!;
    }
}
