namespace IdSubjects.Tests.Fixtures;
public class NaturalPersonMocker
{
    /// <summary>
    /// 创建一个指定UserName和DisplayName的自然人。
    /// </summary>
    /// <param name="manager"></param>
    /// <param name="userName"></param>
    /// <param name="displayName"></param>
    /// <returns></returns>
    public async Task<NaturalPerson> CreateMockPersonAsync(NaturalPersonManager manager, string userName, string displayName)
    {
        var person = new NaturalPerson(userName, new PersonNameInfo(displayName));
        _ = await manager.CreateAsync(person);
        return person;
    }

    /// <summary>
    /// 创建一个UserName是MockPerson, Name是张无忌，其他属性保持默认的自然人。
    /// </summary>
    /// <param name="manager"></param>
    /// <returns></returns>
    public Task<NaturalPerson> CreateDefaultMockPersonAsync(NaturalPersonManager manager) => this.CreateMockPersonAsync(manager, "MockPerson", "张无忌");
}
