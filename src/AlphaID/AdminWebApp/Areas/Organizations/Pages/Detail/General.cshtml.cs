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

        public IdOperationResult? OperationResult { get; set; }

        public async Task<IActionResult> OnGetAsync(string anchor)
        {
            var org = await this.manager.FindByIdAsync(anchor);
            if (org == null)
                return this.NotFound();

            this.Input = new InputModel
            {
                Contact = org.Contact,
                Domicile = org.Domicile,
                Representative = org.Representative,
                Website = org.Website,
                EstablishedAt = org.EstablishedAt?.ToDateTime(TimeOnly.MinValue),
                Name = org.Name,
                TermBegin = org.TermBegin?.ToDateTime(TimeOnly.MinValue),
                TermEnd = org.TermEnd?.ToDateTime(TimeOnly.MinValue),
            };
            return this.Page();
        }

        public async Task<IActionResult> OnPostAsync(string anchor)
        {
            var org = await this.manager.FindByIdAsync(anchor);
            if (org == null)
                return this.NotFound();

            if (!this.ModelState.IsValid)
                return this.Page();

            org.Contact = this.Input.Contact;
            org.Domicile = this.Input.Domicile;
            org.Representative = this.Input.Representative;
            org.Website = this.Input.Website;
            org.EstablishedAt = this.Input.EstablishedAt.HasValue ? DateOnly.FromDateTime(this.Input.EstablishedAt.Value) : null;
            org.TermBegin = this.Input.TermBegin.HasValue ? DateOnly.FromDateTime(this.Input.TermBegin.Value) : null;
            org.TermEnd = this.Input.TermEnd.HasValue ? DateOnly.FromDateTime(this.Input.TermEnd.Value) : null;

            await this.manager.UpdateAsync(org);
            this.OperationResult = IdOperationResult.Success;
            return this.Page();
        }

        public class InputModel
        {
            [Display(Name = "Name")]
            public string Name { get; init; } = default!;

            [Display(Name = "Domicile")]
            public string? Domicile { get; init; }

            [Display(Name = "Contact")]
            public string? Contact { get; init; }

            [Display(Name = "Representative")]
            public string? Representative { get; init; }

            [Display(Name = "Website")]
            public string? Website { get; init; }

            [Display(Name = "Established at")]
            [DataType(DataType.Date)]
            public DateTime? EstablishedAt { get; init; }

            [Display(Name = "Term begin")]
            [DataType(DataType.Date)]
            public DateTime? TermBegin { get; init; }

            [Display(Name = "Term end")]
            [DataType(DataType.Date)]
            public DateTime? TermEnd { get; init; }
        }
    }
}
