using IdSubjects.DirectoryLogon;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace AdminWebApp.Areas.SystemSettings.Pages.DirectoryServices;

public class CreateModel(DirectoryServiceManager directoryServiceManager) : PageModel
{
    [BindProperty]
    public InputModel Input { get; set; } = default!;

    public void OnGet()
    {
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (this.Input is { ExternalProviderName: not null, RegisteredClientId: null }) this.ModelState.AddModelError("Input.RegisteredClientId", "Registered Client-Id is required when External provider specified.");

        if (!this.ModelState.IsValid)
            return this.Page();

        var directoryService = new DirectoryServiceDescriptor()
        {
            Name = this.Input.Name,
            ServerAddress = this.Input.ServerAddress,
            Type= this.Input.LdapType,
            RootDn = this.Input.RootDn,
            DefaultUserAccountContainer = this.Input.DefaultUserOu,
            UpnSuffix = this.Input.UpnSuffix,
            SamDomainPart = this.Input.NTDomainName,
            AutoCreateAccount = this.Input.AutoCreateAccount,
            UserName = this.Input.UserName,
            Password = this.Input.Password,
        };
        if (this.Input.ExternalProviderName != null)
        {
            directoryService.ExternalLoginProvider =
                new ExternalLoginProviderInfo(this.Input.ExternalProviderName, this.Input.RegisteredClientId!)
                {
                    DisplayName = this.Input.ExternalProviderDisplayName,
                    SubjectGenerator = this.Input.SubjectGenerator,
                };
        }

        var result = await directoryServiceManager.CreateAsync(directoryService);
        if (!result.Succeeded)
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
        [Display(Name = "Name")]
        [Required(ErrorMessage = "Validate_Required")]
        [StringLength(50, ErrorMessage = "Validate_StringLength")]
        public string Name { get; set; } = default!;

        [Display(Name = "Server", Prompt = "ldap.example.com")]
        [Required(ErrorMessage = "Validate_Required")]
        [StringLength(50, ErrorMessage = "Validate_StringLength")]
        public string ServerAddress { get; set; } = default!;

        [Display(Name = "LDAP Type")]
        public LdapType LdapType { get; set; }

        [Display(Name = "User name")]
        [StringLength(50, ErrorMessage = "Validate_StringLength")]
        [DataType(DataType.Password)]
        public string? UserName { get; set; }

        [Display(Name = "Password")]
        [StringLength(50, ErrorMessage = "Validate_StringLength")]
        [DataType(DataType.Password)]
        public string? Password { get; set; }

        [Display(Name = "Root DN", Prompt = "DC=example,DC=com")]
        [Required(ErrorMessage = "Validate_Required")]
        [StringLength(150, ErrorMessage = "Validate_StringLength")]
        public string RootDn { get; set; } = default!;

        [Display(Name = "User OU", Prompt = "OU=Users,DC=example,DC=com")]
        [Required(ErrorMessage = "Validate_Required")]
        [StringLength(150, ErrorMessage = "Validate_StringLength")]
        public string DefaultUserOu { get; set; } = default!;

        [Display(Name = "UPN suffix", Prompt = "example.com")]
        [Required(ErrorMessage = "Validate_Required")]
        [StringLength(20, ErrorMessage = "Validate_StringLength")]
        public string UpnSuffix { get; set; } = default!;

        [Display(Name = "NT Domain Name")]
        [StringLength(20, ErrorMessage = "Validate_StringLength")]
        public string? NTDomainName { get; set; }

        [Display(Name = "Auto Create Account")]
        public bool AutoCreateAccount { get; set; } = false;

        [Display(Name = "External Provider Name")]
        [StringLength(20, ErrorMessage = "Validate_StringLength")]
        public string? ExternalProviderName { get; set; }

        [Display(Name = "External Provider Display Name")]
        [StringLength(20, ErrorMessage = "Validate_StringLength")]
        public string? ExternalProviderDisplayName { get; set; }

        [Display(Name = "Registered Client-Id")]
        [StringLength(20, ErrorMessage = "Validate_StringLength")]
        public string? RegisteredClientId { get; set; }

        [Display(Name = "Subject Generator")]
        [StringLength(255, ErrorMessage = "Validate_StringLength")]
        public string? SubjectGenerator { get; set; }
    }
}
