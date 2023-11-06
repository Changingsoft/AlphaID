namespace IDSubjects.ChineseName;

/// <summary>
/// 中国人姓名拼音转换拦截器。
/// </summary>
public interface IChinesePersonNamePinyinInterceptor
{
    /// <summary>
    /// 在转换后执行。
    /// </summary>
    /// <param name="context"></param>
    void AfterConvert(ChineseNamePinyinConvertContext context);
}
