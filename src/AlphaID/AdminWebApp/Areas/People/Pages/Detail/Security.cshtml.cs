using IDSubjects;
using Microsoft.AspNetCore.Mvc;

namespace AdminWebApp.Areas.People.Pages.Detail;

public class SecurityModel : PageModel
{
    private readonly NaturalPersonManager manager;

    public SecurityModel(NaturalPersonManager manager)
    {
        this.manager = manager;
    }

    [BindProperty(SupportsGet = true)]
    public string Anchor { get; set; } = default!;

    [BindProperty]
    public InputModel Input { get; set; } = default!;

    public NaturalPerson Data { get; set; } = default!;

    public string OperationMessage = default!;

    public async Task<IActionResult> OnGetAsync()
    {
        var person = await this.manager.FindByIdAsync(this.Anchor.ToString());
        if (person == null) { return this.NotFound(); }

        this.Data = person;
        this.Input = new InputModel
        {
            Enabled = this.Data.Enabled,
            TwoFactorEnabled = this.Data.TwoFactorEnabled,
            LockoutEnabled = this.Data.LockoutEnabled,
        };
        return this.Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        var person = await this.manager.FindByIdAsync(this.Anchor.ToString());
        if (person == null) { return this.NotFound(); }

        this.Data = person;

        this.Data.Enabled = this.Input.Enabled;
        this.Data.TwoFactorEnabled = this.Input.TwoFactorEnabled;
        this.Data.LockoutEnabled = this.Input.LockoutEnabled;

        await this.manager.UpdateAsync(this.Data);
        this.OperationMessage = "ÒÑ¸üÐÂ";
        return this.Page();
    }

    public class InputModel
    {
        public bool Enabled { get; set; }

        public bool TwoFactorEnabled { get; set; }

        public bool LockoutEnabled { get; set; }


    }
}
