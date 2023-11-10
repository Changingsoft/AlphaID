using AlphaID.EntityFramework;
using IDSubjects.ChineseName;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace DataTool;
internal class PinyinCorrectionWork : BackgroundService
{
    private readonly IServiceScopeFactory factory;
    private readonly ChinesePersonNamePinyinConverter chinesePersonNamePinyinConverter;

    public PinyinCorrectionWork(IServiceScopeFactory factory, ChinesePersonNamePinyinConverter chinesePersonNamePinyinConverter)
    {
        this.factory = factory;
        this.chinesePersonNamePinyinConverter = chinesePersonNamePinyinConverter;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        using var scope = this.factory.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<IdSubjectsDbContext>();
        var count = 0;
        foreach (var person in db.People.Where(p => p.PersonName.FullName.Contains('洋')))
        {
            count++;
            if (person is { PersonName.Surname: not null, PersonName.GivenName: not null })
            {
                var (pinyinSurname, pinyinGivenName) = this.chinesePersonNamePinyinConverter.Convert(person.PersonName.Surname, person.PersonName.GivenName);
                person.PhoneticSurname = pinyinSurname;
                person.PhoneticGivenName = pinyinGivenName;
                person.PersonName.SearchHint = $"{pinyinSurname}{pinyinGivenName}";
            }
            Console.WriteLine($"{count:0000000}");
        }
        Console.WriteLine("Saveing...");
        await db.SaveChangesAsync(stoppingToken);
        Console.WriteLine("DONE!");
    }


}
