using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;

namespace AuthCenterWebApp.Areas.Organization.Pages.Settings
{
    public class FapiaoInfoModel : PageModel
    {

        [BindProperty]
        public InputModel Input { get; set; } = default!;

        public void OnGet()
        {
        }

        public class InputModel
        {
            [Display(Name = "Name")]
            public string Name { get; init; } = default!;

            [Display(Name = "Taxpayer ID")]
            public string TaxpayerId { get; init; } = default!;

            [Display(Name = "Address")]
            public string Address { get; init; } = default!;

            [Display(Name = "Contact")]
            public string Contact { get; init; } = default!;

            [Display(Name = "Bank")]
            public string Bank { get; init; } = default!;

            [Display(Name = "Account")]
            public string Account { get; init; } = default!;
        }
    }
}
