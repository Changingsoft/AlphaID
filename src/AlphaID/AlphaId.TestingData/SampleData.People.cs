using IdSubjects;

namespace AlphaId.TestingData;

/// <content>
/// <see cref="SampleData"/> 的样例自然人。
/// </content>
public static partial class SampleData
{
    /// <summary>曹操（<c>caocao</c>）。</summary>
    public static SamplePerson CaoCao { get; } = new()
    {
        Id = "020d87bb-a337-457c-af6d-53bc2d355579",
        UserName = "caocao",
        FamilyName = "曹",
        GivenName = "操",
        Name = "曹操",
        NickName = "曹阿瞒",
        Bio = "说曹操曹操就到",
        PhoneticSurname = "CAO",
        PhoneticGivenName = "CAO",
        SearchHint = "CAOCAO",
        Gender = Gender.Male,
        DateOfBirth = new DateOnly(155, 7, 18),
        Locale = "zh-CN",
        TimeZone = "Asia/Shanghai",
        WhenCreated = Time("2023-11-07T17:31:44.5959873+00:00"),
        WhenChanged = Time("2025-03-06T08:25:37.5980376+00:00"),
        PasswordHash = SampleData.PasswordHashA,
        SecurityStamp = "HM5YOGVMAANI3UCLM46FNCHVL6SABPSH",
        ConcurrencyStamp = "faf007b1-cb8e-439a-9c04-0502b34b7e66",
        AvatarFileName = "caocao.png",
        AvatarMimeType = "image/png",
    };

    /// <summary>董卓（<c>dongzhuo</c>）。</summary>
    public static SamplePerson DongZhuo { get; } = new()
    {
        Id = "0f7fc2c8-34b6-417e-a4e4-604bd4cbac01",
        UserName = "dongzhuo",
        PhoneNumber = "+8613812340009",
        FamilyName = "董",
        GivenName = "卓",
        Name = "董卓",
        Bio = "尔要尝尝我宝剑之锋利吗？",
        PhoneticSurname = "DONG",
        PhoneticGivenName = "ZHUO",
        SearchHint = "DONGZHUO",
        Gender = Gender.Male,
        Locale = "zh-CN",
        TimeZone = "Asia/Shanghai",
        WhenCreated = Time("2023-11-07T17:36:31.3335603+00:00"),
        WhenChanged = Time("2023-11-09T09:06:13.3407065+00:00"),
        PasswordHash = SampleData.PasswordHashA,
        SecurityStamp = "PAHXYFBH4WNNFBNOE64VXBKYZE2352RQ",
        ConcurrencyStamp = "d0118ab2-21ae-450d-8283-b6c03c790f56",
        AvatarFileName = "dongzhuo.png",
        AvatarMimeType = "image/png",
    };

    /// <summary>孙权（<c>sunquan</c>）。</summary>
    public static SamplePerson SunQuan { get; } = new()
    {
        Id = "1ccfec51-9ad1-456c-8e1e-ce4839d2ca3d",
        UserName = "sunquan",
        PhoneNumber = "+8613812340006",
        FamilyName = "孙",
        GivenName = "权",
        Name = "孙权",
        NickName = "孙十万",
        PhoneticSurname = "SUN",
        PhoneticGivenName = "QUAN",
        SearchHint = "SUNQUAN",
        Gender = Gender.Male,
        Locale = "zh-CN",
        TimeZone = "Asia/Shanghai",
        WhenCreated = Time("2023-11-07T17:32:57.4779702+00:00"),
        WhenChanged = Time("2023-11-09T09:06:36.5408479+00:00"),
        PasswordHash = SampleData.PasswordHashA,
        SecurityStamp = "Y6DOE7VZIXRNFVYNY27VMNQWC6DM4PLE",
        ConcurrencyStamp = "275115be-ce62-4b5a-8119-801b41cdfe39",
        AvatarFileName = "sunquan.png",
        AvatarMimeType = "image/png",
    };

    /// <summary>吕布（<c>lvbu</c>）。</summary>
    public static SamplePerson LvBu { get; } = new()
    {
        Id = "2b8e184c-c6a5-45d7-8132-8aa53eaf3fd4",
        UserName = "lvbu",
        PhoneNumber = "+8613812340007",
        FamilyName = "吕",
        GivenName = "布",
        Name = "吕布",
        Bio = "哎，也是处于无奈啊~",
        PhoneticSurname = "LV",
        PhoneticGivenName = "BU",
        SearchHint = "LVBU",
        Gender = Gender.Male,
        Locale = "zh-CN",
        TimeZone = "Asia/Shanghai",
        WhenCreated = Time("2023-11-07T17:33:44.2038584+00:00"),
        WhenChanged = Time("2023-11-09T09:06:58.0015834+00:00"),
        PasswordHash = SampleData.PasswordHashA,
        SecurityStamp = "S3C2ZCW5WTYUBCRCOMRDQV3Y2SE2IA2T",
        ConcurrencyStamp = "b5360bcc-66e8-4210-9acd-38443ef6a685",
        AvatarFileName = "lvbu.png",
        AvatarMimeType = "image/png",
    };

    /// <summary>花木兰（<c>jjfjj</c>）。</summary>
    public static SamplePerson HuaMuLan { get; } = new()
    {
        Id = "704bdb5a-bf01-4744-b8c2-f9da7cb28933",
        UserName = "jjfjj",
        PhoneNumber = "+8613887451234",
        FamilyName = "花",
        GivenName = "木兰",
        Name = "花木兰",
        PhoneticSurname = "HUA",
        PhoneticGivenName = "MULAN",
        SearchHint = "HUA木兰",
        Gender = Gender.Female,
        WhenCreated = Time("2025-03-06T05:15:56.0850725+00:00"),
        WhenChanged = Time("2025-03-06T05:15:56.0850725+00:00"),
        SecurityStamp = "5NOMFFV64YSIP4KYVW3NW2BBCU4SVRQJ",
        ConcurrencyStamp = "8e8c59ed-360b-43fb-9a33-59258d4a782a",
    };

    /// <summary>周瑜（<c>zhouyu</c>）。</summary>
    public static SamplePerson ZhouYu { get; } = new()
    {
        Id = "77750ac0-813b-4da9-91df-540432117d46",
        UserName = "zhouyu",
        PhoneNumber = "+8613812340012",
        FamilyName = "周",
        GivenName = "瑜",
        Name = "周瑜",
        Bio = "既生瑜，何生亮！",
        PhoneticSurname = "ZHOU",
        PhoneticGivenName = "YU",
        SearchHint = "ZHOUYU",
        Gender = Gender.Male,
        Locale = "zh-CN",
        TimeZone = "Asia/Shanghai",
        WhenCreated = Time("2023-11-07T17:39:42.1628361+00:00"),
        WhenChanged = Time("2023-11-09T09:07:19.1742617+00:00"),
        PasswordHash = SampleData.PasswordHashA,
        SecurityStamp = "OPZKEG4EAC4N63Y5EM5DIVD7OVSZQQKM",
        ConcurrencyStamp = "8ff0b777-ab60-4cd5-a789-324c3eb66274",
        AvatarFileName = "zhouyu.png",
        AvatarMimeType = "image/png",
    };

    /// <summary>孙策（<c>sunce</c>）。</summary>
    public static SamplePerson SunCe { get; } = new()
    {
        Id = "9662c73b-78d3-4bde-96ac-017c0f754b68",
        UserName = "sunce",
        PhoneNumber = "+8613812340011",
        FamilyName = "孙",
        GivenName = "策",
        Name = "孙策",
        PhoneticSurname = "SUN",
        PhoneticGivenName = "CE",
        SearchHint = "SUNCE",
        Gender = Gender.Male,
        Locale = "zh-CN",
        TimeZone = "Asia/Shanghai",
        WhenCreated = Time("2023-11-07T17:39:00.5180856+00:00"),
        WhenChanged = Time("2023-11-09T09:07:49.5669351+00:00"),
        PasswordHash = SampleData.PasswordHashA,
        SecurityStamp = "R43KLY6CAH4ANXLUVR4SPZ4KMKBWIBXM",
        ConcurrencyStamp = "f26367e5-ff51-4684-8aa0-f03ef509e687",
        AvatarFileName = "sunce.png",
        AvatarMimeType = "image/png",
    };

    /// <summary>司马昭（<c>simazhao</c>）。</summary>
    public static SamplePerson SiMaZhao { get; } = new()
    {
        Id = "adbb8d41-2a89-42b0-a272-5e0a9a324bb5",
        UserName = "simazhao",
        PhoneNumber = "+8613812340013",
        FamilyName = "司马",
        GivenName = "昭",
        Name = "司马昭",
        Bio = "我的心路人皆知？",
        PhoneticSurname = "SIMA",
        PhoneticGivenName = "ZHAO",
        SearchHint = "SIMAZHAO",
        Gender = Gender.Male,
        Locale = "zh-CN",
        TimeZone = "Asia/Shanghai",
        WhenCreated = Time("2023-11-07T17:40:52.5805395+00:00"),
        WhenChanged = Time("2023-11-09T09:08:11.2207492+00:00"),
        PasswordHash = SampleData.PasswordHashA,
        SecurityStamp = "W4HGBG6RR3XXBFM7MYV7ZNY63UKPBGV6",
        ConcurrencyStamp = "80a9b3c9-4845-49fb-8d85-67b54a423eb4",
        AvatarFileName = "simazhao.png",
        AvatarMimeType = "image/png",
    };

    /// <summary>关羽（<c>guanyu</c>）。</summary>
    public static SamplePerson GuanYu { get; } = new()
    {
        Id = "bf16436b-d15f-44b7-bd61-831eacee5063",
        UserName = "guanyu",
        Email = "guanyu@sanguo.net",
        PhoneNumber = "+8613812340002",
        EmailConfirmed = true,
        PhoneNumberConfirmed = true,
        FamilyName = "关",
        GivenName = "羽",
        Name = "关羽",
        NickName = "二爷",
        Bio = "竹可焚而不可毁其节，玉可碎而不可改其白",
        PhoneticSurname = "GUAN",
        PhoneticGivenName = "YU",
        SearchHint = "GUANYU",
        Gender = Gender.Male,
        Locale = "zh-CN",
        TimeZone = "Asia/Shanghai",
        WhenCreated = Time("2021-01-20T11:53:34.9433333+00:00"),
        WhenChanged = Time("2023-11-09T09:08:40.1193170+00:00"),
        LockoutEnd = Time("2023-03-03T02:04:42.2746978+00:00"),
        PasswordLastSet = Time("2023-03-03T10:15:24.0019324+00:00"),
        PasswordHash = SampleData.PasswordHashB,
        SecurityStamp = "MQ7QKWU5PV5WFQYHPPTNA2HOEL6DAJNA",
        ConcurrencyStamp = "10170147-e214-41d2-9df5-cff07ca4cc0f",
        AvatarFileName = "guanyu.png",
        AvatarMimeType = "image/png",
        ExternalLogins =
        [
            new() { LoginProvider = "federal.changingsoft.com", ProviderKey = "nWZDRjhYSFDnRF1M8/nSIuCL8ZFAN4YYG0MaCYDP+Sw=", ProviderDisplayName = "AD FS" },
        ],
        BankAccounts =
        [
            new() { AccountNumber = "62284813594021514", AccountName = "刘备", BankName = "西蜀银行" },
        ],
    };

    /// <summary>张飞（<c>zhangfei</c>）。</summary>
    public static SamplePerson ZhangFei { get; } = new()
    {
        Id = "c12c61e6-49a3-4f6d-8cea-7f039ec371a1",
        UserName = "zhangfei",
        Email = "zhangfei@sanguo.net",
        PhoneNumber = "+8613812340003",
        FamilyName = "张",
        GivenName = "飞",
        Name = "张飞",
        NickName = "飞飞",
        Bio = "俺也一样！",
        PhoneticSurname = "ZHANG",
        PhoneticGivenName = "FEI",
        SearchHint = "ZHANGFEI",
        Gender = Gender.Male,
        Locale = "zh-CN",
        TimeZone = "Asia/Shanghai",
        WhenCreated = Time("2022-11-29T15:35:42.4559587+00:00"),
        WhenChanged = Time("2023-11-09T09:09:02.7252691+00:00"),
        PasswordLastSet = Time("2023-03-03T10:15:24.0019324+00:00"),
        PasswordHash = SampleData.PasswordHashB,
        SecurityStamp = "YHPFLCBBAQYRKCIWWQWY3ICDZFMONRJT",
        ConcurrencyStamp = "93eb3dd0-f6f9-44ca-b692-0f2d0f3dce10",
        AvatarFileName = "zhangfei.png",
        AvatarMimeType = "image/png",
    };

    /// <summary>杨修（<c>yangxiu</c>）。</summary>
    public static SamplePerson YangXiu { get; } = new()
    {
        Id = "c2f7cf9b-4dec-4c05-8451-da58c9b0f39f",
        UserName = "yangxiu",
        PhoneNumber = "+8613812340008",
        FamilyName = "杨",
        GivenName = "修",
        Name = "杨修",
        Bio = "一人一口酥",
        PhoneticSurname = "YANG",
        PhoneticGivenName = "XIU",
        SearchHint = "YANGXIU",
        Gender = Gender.Male,
        Locale = "zh-CN",
        TimeZone = "Asia/Shanghai",
        WhenCreated = Time("2023-11-07T17:34:43.1562890+00:00"),
        WhenChanged = Time("2023-11-09T09:09:20.2974228+00:00"),
        PasswordHash = SampleData.PasswordHashA,
        SecurityStamp = "UQV6IG3DABVXMHZBFQOHXNQBZAO7CD6I",
        ConcurrencyStamp = "38330cc2-738a-4a72-a6bb-91343f1f058a",
        AvatarFileName = "yangxiu.png",
        AvatarMimeType = "image/png",
    };

    /// <summary>袁绍（<c>yuanshao</c>）。</summary>
    public static SamplePerson YuanShao { get; } = new()
    {
        Id = "cc9b802c-26ee-409b-ab7f-951f5bd26190",
        UserName = "yuanshao",
        PhoneNumber = "+8613812340010",
        FamilyName = "袁",
        GivenName = "绍",
        Name = "袁绍",
        Bio = "我的剑也未尝不利！",
        PhoneticSurname = "YUAN",
        PhoneticGivenName = "SHAO",
        SearchHint = "YUANSHAO",
        Gender = Gender.Male,
        Locale = "zh-CN",
        TimeZone = "Asia/Shanghai",
        WhenCreated = Time("2023-11-07T17:38:15.4016929+00:00"),
        WhenChanged = Time("2023-11-09T09:09:41.5691794+00:00"),
        PasswordHash = SampleData.PasswordHashA,
        SecurityStamp = "I2HCD33RA4TCIV5Z5EQ7VWI6PUA65K6Z",
        ConcurrencyStamp = "9590047a-94a9-497b-b0ba-d0a61ec2c7d2",
        AvatarFileName = "yuanshao.png",
        AvatarMimeType = "image/png",
    };

    /// <summary>刘备（<c>liubei</c>）。</summary>
    public static SamplePerson LiuBei { get; } = new()
    {
        Id = "d2480421-8a15-4292-8e8f-06985a1f645b",
        UserName = "liubei",
        Email = "liubei@sanguo.net",
        PhoneNumber = "+8613812340001",
        EmailConfirmed = true,
        PhoneNumberConfirmed = true,
        FamilyName = "刘",
        GivenName = "备",
        Name = "刘备",
        NickName = "备备",
        Bio = "吾乃中山靖王之后！",
        PhoneticSurname = "LIU",
        PhoneticGivenName = "BEI",
        SearchHint = "LIUBEI",
        Gender = Gender.Male,
        DateOfBirth = new DateOnly(161, 7, 16),
        Locale = "zh-CN",
        TimeZone = "Asia/Shanghai",
        Address = new AddressInfo { Country = "中国", Region = "河北省", Locality = "涿州市", Street1 = "大树楼桑村", Recipient = "刘备", PostalCode = "072750" },
        WhenCreated = Time("2021-01-20T00:00:00.0000000+00:00"),
        WhenChanged = Time("2025-03-06T02:28:43.3686647+00:00"),
        PasswordLastSet = Time("2025-03-06T02:28:43.2275293+00:00"),
        PasswordHash = SampleData.PasswordHashC,
        SecurityStamp = "4XATZQSGPZ6QJDE4PN2HLKNO224JZ5LW",
        ConcurrencyStamp = "992bbe11-34ac-44d0-bdc2-10d9bf997c2d",
        AvatarFileName = "liubei.jpg",
        AvatarMimeType = "image/jpeg",
        ExternalLogins =
        [
            new() { LoginProvider = "federal.changingsoft.com", ProviderKey = "m9TZEJxoHgsFUmXA5UrFYMVHGmk91QOLFGPGMEcFA3I=", ProviderDisplayName = "员工登录" },
        ],
    };

    /// <summary>诸葛亮（<c>zhugeliang</c>）。</summary>
    public static SamplePerson ZhuGeLiang { get; } = new()
    {
        Id = "f23eef91-e089-4164-99a7-909cdae5eac7",
        UserName = "zhugeliang",
        PhoneNumber = "+8613812340004",
        FamilyName = "诸葛",
        GivenName = "亮",
        Name = "诸葛亮",
        NickName = "亮亮",
        PhoneticSurname = "ZHUGE",
        PhoneticGivenName = "LIANG",
        SearchHint = "ZHUGELIANG",
        Gender = Gender.Male,
        DateOfBirth = new DateOnly(181, 8, 20),
        Locale = "zh-CN",
        TimeZone = "Asia/Shanghai",
        Address = new AddressInfo { Country = "中国", Region = "徐州", Locality = "琅琊", Street1 = "阳都", Recipient = "诸葛亮" },
        WhenCreated = Time("2023-11-07T17:27:59.3057017+00:00"),
        WhenChanged = Time("2023-11-09T09:10:03.4105181+00:00"),
        PasswordHash = SampleData.PasswordHashA,
        SecurityStamp = "5QHHWWZPYW2IQBFE2P6XW7W3EFWLLTKC",
        ConcurrencyStamp = "b1775762-6877-4cea-b982-842837f3c133",
        AvatarFileName = "zhugeliang.png",
        AvatarMimeType = "image/png",
    };

    /// <summary>
    /// 全部样例自然人。
    /// </summary>
    /// <remarks>
    /// 顺序与原 T-SQL 种子脚本一致（按 <see cref="SamplePerson.Id"/> 升序）。
    /// <c>UsedPassword.Id</c> 是自增列，其取值由写入顺序决定，保留顺序才能让种子结果逐字节可比。
    /// </remarks>
    public static IReadOnlyList<SamplePerson> People =>
    [
        CaoCao, DongZhuo, SunQuan, LvBu,
        HuaMuLan, ZhouYu, SunCe, SiMaZhao,
        GuanYu, ZhangFei, YangXiu, YuanShao,
        LiuBei, ZhuGeLiang,
    ];
}
