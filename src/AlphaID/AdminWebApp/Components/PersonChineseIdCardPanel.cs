using IDSubjects;
using IDSubjects.RealName;
using Microsoft.AspNetCore.Mvc;

namespace AdminWebApp.Components;

public class PersonChineseIdCardPanel : ViewComponent
{
    private readonly ChineseIdCardManager chineseIdCardManager;

    public PersonChineseIdCardPanel(ChineseIdCardManager chineseIdCardManager)
    {
        this.chineseIdCardManager = chineseIdCardManager;
    }

    public async Task<IViewComponentResult> InvokeAsync(NaturalPerson person)
    {
        var validation = await this.chineseIdCardManager.GetCurrentAsync(person);
        return this.View(validation);
    }
}
