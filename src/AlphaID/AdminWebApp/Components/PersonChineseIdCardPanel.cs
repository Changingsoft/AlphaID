using IDSubjects;
using IDSubjects.RealName;
using Microsoft.AspNetCore.Mvc;

namespace AdminWebApp.Components;

public class PersonChineseIdCardPanel : ViewComponent
{
    private readonly ChineseIDCardManager chineseIDCardManager;

    public PersonChineseIdCardPanel(ChineseIDCardManager chineseIDCardManager)
    {
        this.chineseIDCardManager = chineseIDCardManager;
    }

    public async Task<IViewComponentResult> InvokeAsync(NaturalPerson person)
    {
        var validation = await this.chineseIDCardManager.GetCurrentAsync(person);
        return this.View(validation);
    }
}
