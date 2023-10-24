using IDSubjects;
using IDSubjects.Subjects;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace AdminWebApp.Areas.Organizations.Pages.Detail
{
    public class GeneralModel : PageModel
    {
        private readonly OrganizationManager manager;

        public GeneralModel(OrganizationManager manager)
        {
            this.manager = manager;
        }

        [BindProperty]
        public InputModel Input { get; set; } = default!;

        public OperationResult? OperationResult { get; set; }

        public async Task<IActionResult> OnGetAsync(string id)
        {
            var org = await this.manager.FindByIdAsync(id);
            if (org == null)
                return this.NotFound();

            this.Input = new()
            {
                Contact = org.Contact,
                Domicile = org.Domicile,
                Representative = org.LegalPersonName,
                USCI = org.USCI,
                Website = org.Website,
                EstablishedAt = org.EstablishedAt,
                Name = org.Name,
                TermBegin = org.TermBegin,
                TermEnd = org.TermEnd,
            };
            return this.Page();
        }

        public async Task<IActionResult> OnPostAsync(string id)
        {
            var org = await this.manager.FindByIdAsync(id);
            if (org == null)
                return this.NotFound();

            USCC usci = default!;
            if (this.Input.USCI != null && !USCC.TryParse(this.Input.USCI, out usci))
                this.ModelState.AddModelError("Input.USCI", "Invalid USCI");

            if (!this.ModelState.IsValid)
                return this.Page();

            org.Contact = this.Input.Contact;
            org.Domicile = this.Input.Domicile;
            org.LegalPersonName = this.Input.Representative;
            org.USCI = this.Input.USCI != null ? usci.ToString() : null;
            org.Website = this.Input.Website;
            org.EstablishedAt = this.Input.EstablishedAt;
            org.TermBegin = this.Input.TermBegin;
            org.TermEnd = this.Input.TermEnd;

            await this.manager.UpdateAsync(org);
            this.OperationResult = OperationResult.Success;
            return this.Page();
        }

        public class InputModel
        {
            [Display(Name = "Name")]
            public string Name { get; set; } = default!;

            [Display(Name = "Domicile")]
            public string? Domicile { get; set; }

            [Display(Name = "USCI")]
            public string? USCI { get; set; }

            [Display(Name = "Contact")]
            public string? Contact { get; set; }

            [Display(Name = "Representative")]
            public string? Representative { get; set; }

            [Display(Name = "Website")]
            public string? Website { get; set; }

            [Display(Name = "Established at")]
            [DataType(DataType.Date)]
            public DateTime? EstablishedAt { get; set; }

            [Display(Name = "Term begin")]
            [DataType(DataType.Date)]
            public DateTime? TermBegin { get; set; }

            [Display(Name = "Term end")]
            [DataType(DataType.Date)]
            public DateTime? TermEnd { get; set; }
        }
    }
}
