namespace AlphaId.TestingData;

/// <content>
/// <see cref="SampleData"/> 的样例组织。
/// </content>
public static partial class SampleData
{
    /// <summary>北魏集团（<c>北魏集团</c>）。</summary>
    public static SampleOrganization BeiWei { get; } = new()
    {
        Id = "1c86b543-0c92-4cd8-bcd5-b4e462847e59",
        Name = "北魏集团",
        Domicile = "许都",
        Contact = "0374-88910001",
        Representative = "曹操",
        EstablishedAt = new DateOnly(1996, 10, 7),
        WhenCreated = Time("2021-01-20T11:56:28.5700000+00:00"),
        WhenChanged = Time("2021-11-22T18:10:24.7755914+00:00"),
        Members =
        [
            new() { PersonId = "020d87bb-a337-457c-af6d-53bc2d355579", IsOwner = true },
        ],
    };

    /// <summary>东吴集团（<c>东吴集团</c>）。</summary>
    public static SampleOrganization DongWu { get; } = new()
    {
        Id = "5288b813-e1f4-4fd3-a342-6f21a4c3fef7",
        Name = "东吴集团",
        Domicile = "建业",
        Contact = "025-20481536",
        Representative = "孙权",
        WhenCreated = Time("2023-03-08T04:41:44.3403565+00:00"),
        WhenChanged = Time("2023-03-08T06:00:26.7989437+00:00"),
        Members =
        [
            new() { PersonId = "1ccfec51-9ad1-456c-8e1e-ce4839d2ca3d", IsOwner = true },
            new() { PersonId = "9662c73b-78d3-4bde-96ac-017c0f754b68" },
        ],
        UsedNames =
        [
            new() { Name = "改名前的有限公司", DeprecateTime = new DateOnly(2022, 6, 1) },
        ],
    };

    /// <summary>蜀汉集团（<c>蜀汉集团</c>）。</summary>
    public static SampleOrganization ShuHan { get; } = new()
    {
        Id = "a7be43af-8b49-450e-a600-90a8748e48a5",
        Name = "蜀汉集团",
        Domicile = "成都",
        Contact = "028-76008888",
        Representative = "刘备",
        // 经纬度逐年逐位照抄历史数据，不要「取整成好看的数字」：
        // geography 以二进制保存坐标，改动小数位就会与历史库不再逐字节一致。
        Location = new SampleLocation(104.140346772, 30.674521447),
        WhenCreated = Time("2023-03-08T04:41:44.3403565+00:00"),
        WhenChanged = Time("2023-03-08T06:00:26.7989437+00:00"),
        Members =
        [
            new() { PersonId = "bf16436b-d15f-44b7-bd61-831eacee5063", Title = "汉寿亭侯" },
            new() { PersonId = "c12c61e6-49a3-4f6d-8cea-7f039ec371a1", Title = "将军" },
            new() { PersonId = "d2480421-8a15-4292-8e8f-06985a1f645b", Title = "皇帝", IsOwner = true },
            new() { PersonId = "f23eef91-e089-4164-99a7-909cdae5eac7", Title = "丞相" },
        ],
    };

    /// <summary>诸葛丞相府管理公司（<c>诸葛丞相府管理公司</c>）。</summary>
    public static SampleOrganization ZhuGeChengXiangFu { get; } = new()
    {
        Id = "c50f590b-5757-4e64-aaf8-0c924dc4e912",
        Name = "诸葛丞相府管理公司",
        Domicile = "四川省成都市浆洗街28号",
        Contact = "028-76152421",
        Representative = "潘益",
        WhenCreated = Time("2023-11-07T17:22:54.0462661+00:00"),
        WhenChanged = Time("2023-11-07T17:22:54.0462661+00:00"),
    };

    /// <summary>全部样例组织。</summary>
    public static IReadOnlyList<SampleOrganization> Organizations =>
    [
        BeiWei, DongWu, ShuHan, ZhuGeChengXiangFu,
    ];
}
