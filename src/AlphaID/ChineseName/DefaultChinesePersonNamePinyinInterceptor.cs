// ReSharper disable StringLiteralTypo

namespace ChineseName;

internal class DefaultChinesePersonNamePinyinInterceptor : IChinesePersonNamePinyinInterceptor
{
    public void AfterConvert(ChineseNamePinyinConvertContext context)
    {
        ShiSelect(context);
    }

    private void ShiSelect(ChineseNamePinyinConvertContext context)
    {
        //单
        PhoneticChineseChar? c = context.SurnameChars.FirstOrDefault(p => p.Origin == '单');
        c?.Select("shan");

        //查
        PhoneticChineseChar? zha = context.SurnameChars.FirstOrDefault(p => p.Origin == '查');
        zha?.Select("zha");

        //许
        foreach (PhoneticChineseChar q in context.SurnameChars.Where(chineseChar => chineseChar.Origin == '许'))
            q.Select("xu");
        foreach (PhoneticChineseChar q in context.GivenNameChars.Where(chineseChar => chineseChar.Origin == '许'))
            q.Select("xu");

        //晟
        foreach (PhoneticChineseChar q in context.SurnameChars.Where(chineseChar => chineseChar.Origin == '晟'))
            q.Select("sheng");
        foreach (PhoneticChineseChar q in context.GivenNameChars.Where(chineseChar => chineseChar.Origin == '晟'))
            q.Select("sheng");

        //石
        foreach (PhoneticChineseChar q in context.SurnameChars.Where(chineseChar => chineseChar.Origin == '石'))
            q.Select("shi");
        foreach (PhoneticChineseChar q in context.GivenNameChars.Where(chineseChar => chineseChar.Origin == '石'))
            q.Select("shi");

        //强
        foreach (PhoneticChineseChar q in context.SurnameChars.Where(chineseChar => chineseChar.Origin == '强'))
            q.Select("qiang");
        foreach (PhoneticChineseChar q in context.GivenNameChars.Where(chineseChar => chineseChar.Origin == '强'))
            q.Select("qiang");

        //卜
        PhoneticChineseChar? bu = context.SurnameChars.FirstOrDefault(p => p.Origin == '卜');
        bu?.Select("bu");

        //陆
        foreach (PhoneticChineseChar q in context.SurnameChars.Where(chineseChar => chineseChar.Origin == '陆'))
            q.Select("lu");
        foreach (PhoneticChineseChar q in context.GivenNameChars.Where(chineseChar => chineseChar.Origin == '陆'))
            q.Select("lu");

        //奇
        foreach (PhoneticChineseChar q in context.SurnameChars.Where(chineseChar => chineseChar.Origin == '奇'))
            q.Select("qi");
        foreach (PhoneticChineseChar q in context.GivenNameChars.Where(chineseChar => chineseChar.Origin == '奇'))
            q.Select("qi");

        //叶
        foreach (PhoneticChineseChar q in context.SurnameChars.Where(chineseChar => chineseChar.Origin == '叶'))
            q.Select("ye");
        foreach (PhoneticChineseChar q in context.GivenNameChars.Where(chineseChar => chineseChar.Origin == '叶'))
            q.Select("ye");

        //曾
        foreach (PhoneticChineseChar q in context.SurnameChars.Where(chineseChar => chineseChar.Origin == '曾'))
            q.Select("zeng");
        foreach (PhoneticChineseChar q in context.GivenNameChars.Where(chineseChar => chineseChar.Origin == '曾'))
            q.Select("zeng");

        //盛
        foreach (PhoneticChineseChar q in context.SurnameChars.Where(chineseChar => chineseChar.Origin == '盛'))
            q.Select("sheng");
        foreach (PhoneticChineseChar q in context.GivenNameChars.Where(chineseChar => chineseChar.Origin == '盛'))
            q.Select("sheng");

        //万
        foreach (PhoneticChineseChar q in context.SurnameChars.Where(chineseChar => chineseChar.Origin == '万'))
            q.Select("wan");
        foreach (PhoneticChineseChar q in context.GivenNameChars.Where(chineseChar => chineseChar.Origin == '万'))
            q.Select("wan");

        //底
        foreach (PhoneticChineseChar q in context.SurnameChars.Where(chineseChar => chineseChar.Origin == '底'))
            q.Select("di");
        foreach (PhoneticChineseChar q in context.GivenNameChars.Where(chineseChar => chineseChar.Origin == '底'))
            q.Select("di");

        //地
        foreach (PhoneticChineseChar q in context.SurnameChars.Where(chineseChar => chineseChar.Origin == '地'))
            q.Select("di");
        foreach (PhoneticChineseChar q in context.GivenNameChars.Where(chineseChar => chineseChar.Origin == '地'))
            q.Select("di");

        //都
        foreach (PhoneticChineseChar q in context.SurnameChars.Where(chineseChar => chineseChar.Origin == '都'))
            q.Select("du");
        foreach (PhoneticChineseChar q in context.GivenNameChars.Where(chineseChar => chineseChar.Origin == '都'))
            q.Select("du");

        //夯
        foreach (PhoneticChineseChar q in context.SurnameChars.Where(chineseChar => chineseChar.Origin == '夯'))
            q.Select("hang");
        foreach (PhoneticChineseChar q in context.GivenNameChars.Where(chineseChar => chineseChar.Origin == '夯'))
            q.Select("hang");

        //合
        foreach (PhoneticChineseChar q in context.SurnameChars.Where(chineseChar => chineseChar.Origin == '合'))
            q.Select("he");
        foreach (PhoneticChineseChar q in context.GivenNameChars.Where(chineseChar => chineseChar.Origin == '合'))
            q.Select("he");

        //红
        foreach (PhoneticChineseChar q in context.SurnameChars.Where(chineseChar => chineseChar.Origin == '红'))
            q.Select("hong");
        foreach (PhoneticChineseChar q in context.GivenNameChars.Where(chineseChar => chineseChar.Origin == '红'))
            q.Select("hong");

        //家
        foreach (PhoneticChineseChar q in context.SurnameChars.Where(chineseChar => chineseChar.Origin == '家'))
            q.Select("jia");
        foreach (PhoneticChineseChar q in context.GivenNameChars.Where(chineseChar => chineseChar.Origin == '家'))
            q.Select("jia");

        //解
        foreach (PhoneticChineseChar q in context.SurnameChars.Where(chineseChar => chineseChar.Origin == '解'))
            q.Select("xie");
        foreach (PhoneticChineseChar q in context.GivenNameChars.Where(chineseChar => chineseChar.Origin == '解'))
            q.Select("xie");

        //落
        foreach (PhoneticChineseChar q in context.SurnameChars.Where(chineseChar => chineseChar.Origin == '落'))
            q.Select("luo");
        foreach (PhoneticChineseChar q in context.GivenNameChars.Where(chineseChar => chineseChar.Origin == '落'))
            q.Select("luo");

        //南
        foreach (PhoneticChineseChar q in context.SurnameChars.Where(chineseChar => chineseChar.Origin == '南'))
            q.Select("nan");
        foreach (PhoneticChineseChar q in context.GivenNameChars.Where(chineseChar => chineseChar.Origin == '南'))
            q.Select("nan");

        //迫
        foreach (PhoneticChineseChar q in context.SurnameChars.Where(chineseChar => chineseChar.Origin == '迫'))
            q.Select("po");
        foreach (PhoneticChineseChar q in context.GivenNameChars.Where(chineseChar => chineseChar.Origin == '迫'))
            q.Select("po");

        //其
        foreach (PhoneticChineseChar q in context.SurnameChars.Where(chineseChar => chineseChar.Origin == '其'))
            q.Select("qi");
        foreach (PhoneticChineseChar q in context.GivenNameChars.Where(chineseChar => chineseChar.Origin == '其'))
            q.Select("qi");

        //且
        foreach (PhoneticChineseChar q in context.SurnameChars.Where(chineseChar => chineseChar.Origin == '且'))
            q.Select("qie");
        foreach (PhoneticChineseChar q in context.GivenNameChars.Where(chineseChar => chineseChar.Origin == '且'))
            q.Select("qie");

        //婼
        foreach (PhoneticChineseChar q in context.SurnameChars.Where(chineseChar => chineseChar.Origin == '婼'))
            q.Select("ruo");
        foreach (PhoneticChineseChar q in context.GivenNameChars.Where(chineseChar => chineseChar.Origin == '婼'))
            q.Select("ruo");

        //呷
        foreach (PhoneticChineseChar q in context.SurnameChars.Where(chineseChar => chineseChar.Origin == '呷'))
            q.Select("xia");
        foreach (PhoneticChineseChar q in context.GivenNameChars.Where(chineseChar => chineseChar.Origin == '呷'))
            q.Select("xia");

        //选
        foreach (PhoneticChineseChar q in context.SurnameChars.Where(chineseChar => chineseChar.Origin == '选'))
            q.Select("xuan");
        foreach (PhoneticChineseChar q in context.GivenNameChars.Where(chineseChar => chineseChar.Origin == '选'))
            q.Select("xuan");

        //转
        foreach (PhoneticChineseChar q in context.SurnameChars.Where(chineseChar => chineseChar.Origin == '转'))
            q.Select("zhuan");
        foreach (PhoneticChineseChar q in context.GivenNameChars.Where(chineseChar => chineseChar.Origin == '转'))
            q.Select("zhuan");

        //瞿
        foreach (PhoneticChineseChar q in context.SurnameChars.Where(chineseChar => chineseChar.Origin == '瞿'))
            q.Select("qu");
        foreach (PhoneticChineseChar q in context.GivenNameChars.Where(chineseChar => chineseChar.Origin == '瞿'))
            q.Select("qu");

        //洋
        foreach (PhoneticChineseChar q in context.SurnameChars.Where(chineseChar => chineseChar.Origin == '洋'))
            q.Select("yang");
        foreach (PhoneticChineseChar q in context.GivenNameChars.Where(chineseChar => chineseChar.Origin == '洋'))
            q.Select("yang");
    }
}