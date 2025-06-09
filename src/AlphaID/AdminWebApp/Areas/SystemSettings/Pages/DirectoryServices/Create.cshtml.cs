using System.ComponentModel.DataAnnotations;
using IdSubjects;
using IdSubjects.DirectoryLogon;
using Microsoft.AspNetCore.Mvc;

namespace AdminWebApp.Areas.SystemSettings.Pages.DirectoryServices;

public class CreateModel(DirectoryServiceManager directoryServiceManager) : PageModel
{
    [BindProperty]
    public InputModel Input { get; set; } = null!;

    public void OnGet()
    {
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (Input is { ExternalProviderName: not null, RegisteredClientId: null })
            ModelState.AddModelError("Input.RegisteredClientId",
                "Registered Client-Id is required when External provider specified.");

        if (!ModelState.IsValid)
            return Page();

        var directoryService = new IdSubjects.DirectoryLogon.DirectoryService
        {
            Name = Input.Name,
            ServerAddress = Input.ServerAddress,
            Type = Input.LdapType,
            RootDn = Input.RootDn,
            DefaultUserAccountContainer = Input.DefaultUserOu,
            UpnSuffix = Input.UpnSuffix,
            SamDomainPart = Input.NtDomainName,
            AutoCreateAccount = Input.AutoCreateAccount,
            UserName = Input.UserName,
            Password = Input.Password
        };
        if (Input.ExternalProviderName != null)
            directoryService.ExternalLoginProvider =
                new ExternalLoginProviderInfo(Input.ExternalProviderName, Input.RegisteredClientId!)
                {
                    DisplayName = Input.ExternalProviderDisplayName,
                    SubjectGenerator = Input.SubjectGenerator
                };

        IdOperationResult result = await directoryServiceManager.CreateAsync(directoryService);
        if (!result.Succeeded)
        {
            foreach (string error in result.Errors) ModelState.AddModelError("", error);
            return Page();
        }

        return RedirectToPage("Index");
    }

    public class InputModel
    {
        [Display(Name = "Name")]
        [Required(ErrorMessage = "Validate_Required")]
        [StringLength(50, ErrorMessage = "Validate_StringLength")]
        public string Name { get; set; } = null!;

        [Display(Name = "Server", Prompt = "ldap.example.com")]
        [Required(ErrorMessage = "Validate_Required")]
        [StringLength(50, ErrorMessage = "Validate_StringLength")]
        public string ServerAddress { get; set; } = null!;

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
        public string RootDn { get; set; } = null!;

        [Display(Name = "User OU", Prompt = "OU=Users,DC=example,DC=com")]
        [Required(ErrorMessage = "Validate_Required")]
        [StringLength(150, ErrorMessage = "Validate_StringLength")]
        public string DefaultUserOu { get; set; } = null!;

        [Display(Name = "UPN suffix", Prompt = "example.com")]
        [Required(ErrorMessage = "Validate_Required")]
        [StringLength(20, ErrorMessage = "Validate_StringLength")]
        public string UpnSuffix { get; set; } = null!;

        [Display(Name = "NT Domain Name")]
        [StringLength(20, ErrorMessage = "Validate_StringLength")]
        public string? NtDomainName { get; set; }

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