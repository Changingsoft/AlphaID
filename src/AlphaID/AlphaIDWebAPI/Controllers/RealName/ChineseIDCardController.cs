using AlphaIdWebAPI.Models;
using IdSubjects;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AlphaIdWebAPI.Controllers.RealName;

/// <summary>
/// 实名信息。
/// </summary>
[Route("api/RealName/[controller]")]
[ApiController]
[Authorize("RealNameScopeRequired")]
public class ChineseIdCardController : ControllerBase
{
    private readonly NaturalPersonManager personManager;
    private readonly ILogger<ChineseIdCardController> logger;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="personManager"></param>
    /// <param name="logger"></param>
    public ChineseIdCardController(NaturalPersonManager personManager, ILogger<ChineseIdCardController> logger)
    {
        this.personManager = personManager;
        this.logger = logger;
    }

    /// <summary>
    /// 获取有关自然人的实名信息。
    /// </summary>
    /// <param name="personId"></param>
    /// <returns></returns>
    [HttpGet("{personId}")]
    public Task<ActionResult<ChineseIdCardModel>> GetChineseIdCardInfo(string personId)
    {
        throw new NotImplementedException();
    }
}
