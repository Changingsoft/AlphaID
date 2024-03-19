using IdSubjects;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace AdminWebApp.Areas.Organizations.Pages.Detail
{
    public class GeneralModel(OrganizationManager manager) : PageModel
    {
        [BindProperty]
        public InputModel Input { get; set; } = default!;

        public IdOperationResult? OperationResult { get; set; }

        public async Task<IActionResult> OnGetAsync(string anchor)
        {
            var org = await manager.FindByIdAsync(anchor);
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
            var org = await manager.FindByIdAsync(anchor);
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

            await manager.UpdateAsync(org);
            this.OperationResult = IdOperationResult.Success;
            return this.Page();
        }

        public class InputModel
        {
            [Display(Name = "Name")]
            [Required(ErrorMessage = "Validate_Required")]
            public string Name { get; set; } = default!;

            [Display(Name = "Domicile")]
            public string? Domicile { get; set; }

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
