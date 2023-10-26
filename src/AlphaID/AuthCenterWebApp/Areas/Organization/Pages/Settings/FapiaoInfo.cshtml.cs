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
            public string Name { get; set; } = default!;

            [Display(Name = "Taxpayer ID")]
            public string TaxpayerId { get; set; } = default!;

            [Display(Name = "Address")]
            public string Address { get; set; } = default!;

            [Display(Name = "Contact")]
            public string Contact { get; set; } = default!;

            [Display(Name = "Bank")]
            public string Bank { get; set; } = default!;

            [Display(Name = "Account")]
            public string Account { get; set; } = default!;
        }
    }
}
