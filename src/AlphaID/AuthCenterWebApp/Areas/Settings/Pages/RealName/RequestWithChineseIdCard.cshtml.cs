using IdSubjects.RealName;
using IdSubjects.RealName.Requesting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AuthCenterWebApp.Areas.Settings.Pages.RealName
{
    public class RequestWithChineseIdCardModel : PageModel
    {

        public RequestWithChineseIdCardModel()
        {
        }

        [BindProperty]
        public IFormFile PersonalSide { get; set; } = default!;

        [BindProperty]
        public IFormFile IssuerSide { get; set; } = default!;

        public void OnGet()
        {
        }

        public async void OnPost()
        {

            var request = new RealNameValidationRequest() { };
        }

        
    }
}
