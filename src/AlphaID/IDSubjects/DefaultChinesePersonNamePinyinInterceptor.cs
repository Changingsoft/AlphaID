namespace IDSubjects;

internal class DefaultChinesePersonNamePinyinInterceptor : IChinesePersonNamePinyinInterceptor
{
    public void AfterConvert(ChineseNamePinyinConvertContext context)
    {
        this.ShiSelect(context);
    }

    private void ShiSelect(ChineseNamePinyinConvertContext context)
    {
        //单
        var c = context.SurnameChars.FirstOrDefault(p => p.Origin == '单');
        c?.Select("shan");

        //查
        var zha = context.SurnameChars.FirstOrDefault(p => p.Origin == '查');
        zha?.Select("zha");

        //许
        foreach (var q in context.SurnameChars.Where(c => c.Origin == '许'))
        {
            q.Select("xu");
        }
        foreach (var q in context.GivenNameChars.Where(c => c.Origin == '许'))
        {
            q.Select("xu");
        }

        //晟
        foreach (var q in context.SurnameChars.Where(c => c.Origin == '晟'))
        {
            q.Select("sheng");
        }
        foreach (var q in context.GivenNameChars.Where(c => c.Origin == '晟'))
        {
            q.Select("sheng");
        }

        //石
        foreach (var q in context.SurnameChars.Where(c => c.Origin == '石'))
        {
            q.Select("shi");
        }
        foreach (var q in context.GivenNameChars.Where(c => c.Origin == '石'))
        {
            q.Select("shi");
        }

        //强
        foreach (var q in context.SurnameChars.Where(c => c.Origin == '强'))
        {
            q.Select("qiang");
        }
        foreach (var q in context.GivenNameChars.Where(c => c.Origin == '强'))
        {
            q.Select("qiang");
        }

        //卜
        var bu = context.SurnameChars.FirstOrDefault(p => p.Origin == '卜');
        bu?.Select("bu");

        //陆
        foreach (var q in context.SurnameChars.Where(c => c.Origin == '陆'))
        {
            q.Select("lu");
        }
        foreach (var q in context.GivenNameChars.Where(c => c.Origin == '陆'))
        {
            q.Select("lu");
        }

        //奇
        foreach (var q in context.SurnameChars.Where(c => c.Origin == '奇'))
        {
            q.Select("qi");
        }
        foreach (var q in context.GivenNameChars.Where(c => c.Origin == '奇'))
        {
            q.Select("qi");
        }

        //叶
        foreach (var q in context.SurnameChars.Where(c => c.Origin == '叶'))
        {
            q.Select("ye");
        }
        foreach (var q in context.GivenNameChars.Where(c => c.Origin == '叶'))
        {
            q.Select("ye");
        }

        //曾
        foreach (var q in context.SurnameChars.Where(c => c.Origin == '曾'))
        {
            q.Select("zeng");
        }
        foreach (var q in context.GivenNameChars.Where(c => c.Origin == '曾'))
        {
            q.Select("zeng");
        }

        //盛
        foreach (var q in context.SurnameChars.Where(c => c.Origin == '盛'))
        {
            q.Select("sheng");
        }
        foreach (var q in context.GivenNameChars.Where(c => c.Origin == '盛'))
        {
            q.Select("sheng");
        }

        //万
        foreach (var q in context.SurnameChars.Where(c => c.Origin == '万'))
        {
            q.Select("wan");
        }
        foreach (var q in context.GivenNameChars.Where(c => c.Origin == '万'))
        {
            q.Select("wan");
        }

        //底
        foreach (var q in context.SurnameChars.Where(c => c.Origin == '底'))
        {
            q.Select("di");
        }
        foreach (var q in context.GivenNameChars.Where(c => c.Origin == '底'))
        {
            q.Select("di");
        }

        //地
        foreach (var q in context.SurnameChars.Where(c => c.Origin == '地'))
        {
            q.Select("di");
        }
        foreach (var q in context.GivenNameChars.Where(c => c.Origin == '地'))
        {
            q.Select("di");
        }

        //都
        foreach (var q in context.SurnameChars.Where(c => c.Origin == '都'))
        {
            q.Select("du");
        }
        foreach (var q in context.GivenNameChars.Where(c => c.Origin == '都'))
        {
            q.Select("du");
        }

        //夯
        foreach (var q in context.SurnameChars.Where(c => c.Origin == '夯'))
        {
            q.Select("hang");
        }
        foreach (var q in context.GivenNameChars.Where(c => c.Origin == '夯'))
        {
            q.Select("hang");
        }

        //合
        foreach (var q in context.SurnameChars.Where(c => c.Origin == '合'))
        {
            q.Select("he");
        }
        foreach (var q in context.GivenNameChars.Where(c => c.Origin == '合'))
        {
            q.Select("he");
        }

        //红
        foreach (var q in context.SurnameChars.Where(c => c.Origin == '红'))
        {
            q.Select("hong");
        }
        foreach (var q in context.GivenNameChars.Where(c => c.Origin == '红'))
        {
            q.Select("hong");
        }

        //家
        foreach (var q in context.SurnameChars.Where(c => c.Origin == '家'))
        {
            q.Select("jia");
        }
        foreach (var q in context.GivenNameChars.Where(c => c.Origin == '家'))
        {
            q.Select("jia");
        }

        //解
        foreach (var q in context.SurnameChars.Where(c => c.Origin == '解'))
        {
            q.Select("xie");
        }
        foreach (var q in context.GivenNameChars.Where(c => c.Origin == '解'))
        {
            q.Select("xie");
        }

        //落
        foreach (var q in context.SurnameChars.Where(c => c.Origin == '落'))
        {
            q.Select("luo");
        }
        foreach (var q in context.GivenNameChars.Where(c => c.Origin == '落'))
        {
            q.Select("luo");
        }

        //南
        foreach (var q in context.SurnameChars.Where(c => c.Origin == '南'))
        {
            q.Select("nan");
        }
        foreach (var q in context.GivenNameChars.Where(c => c.Origin == '南'))
        {
            q.Select("nan");
        }

        //迫
        foreach (var q in context.SurnameChars.Where(c => c.Origin == '迫'))
        {
            q.Select("po");
        }
        foreach (var q in context.GivenNameChars.Where(c => c.Origin == '迫'))
        {
            q.Select("po");
        }

        //其
        foreach (var q in context.SurnameChars.Where(c => c.Origin == '其'))
        {
            q.Select("qi");
        }
        foreach (var q in context.GivenNameChars.Where(c => c.Origin == '其'))
        {
            q.Select("qi");
        }

        //且
        foreach (var q in context.SurnameChars.Where(c => c.Origin == '且'))
        {
            q.Select("qie");
        }
        foreach (var q in context.GivenNameChars.Where(c => c.Origin == '且'))
        {
            q.Select("qie");
        }

        //婼
        foreach (var q in context.SurnameChars.Where(c => c.Origin == '婼'))
        {
            q.Select("ruo");
        }
        foreach (var q in context.GivenNameChars.Where(c => c.Origin == '婼'))
        {
            q.Select("ruo");
        }

        //呷
        foreach (var q in context.SurnameChars.Where(c => c.Origin == '呷'))
        {
            q.Select("xia");
        }
        foreach (var q in context.GivenNameChars.Where(c => c.Origin == '呷'))
        {
            q.Select("xia");
        }

        //选
        foreach (var q in context.SurnameChars.Where(c => c.Origin == '选'))
        {
            q.Select("xuan");
        }
        foreach (var q in context.GivenNameChars.Where(c => c.Origin == '选'))
        {
            q.Select("xuan");
        }

        //转
        foreach (var q in context.SurnameChars.Where(c => c.Origin == '转'))
        {
            q.Select("zhuan");
        }
        foreach (var q in context.GivenNameChars.Where(c => c.Origin == '转'))
        {
            q.Select("zhuan");
        }

        //瞿
        foreach (var q in context.SurnameChars.Where(c => c.Origin == '瞿'))
        {
            q.Select("qu");
        }
        foreach (var q in context.GivenNameChars.Where(c => c.Origin == '瞿'))
        {
            q.Select("qu");
        }

        //洋
        foreach (var q in context.SurnameChars.Where(c => c.Origin == '洋'))
        {
            q.Select("yang");
        }
        foreach (var q in context.GivenNameChars.Where(c => c.Origin == '洋'))
        {
            q.Select("yang");
        }
    }
}
