using AlphaIDWebAPI.Models;
using IDSubjects;
using IDSubjects.RealName;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AlphaIDWebAPI.Controllers.RealName;

/// <summary>
/// 实名信息。
/// </summary>
[Route("api/RealName/[controller]")]
[ApiController]
[Authorize("RealNameScopeRequired")]
public class ChineseIDCardController : ControllerBase
{
    private readonly NaturalPersonManager personManager;
    private readonly ChineseIDCardManager chineseIDCardManager;
    private readonly ILogger<ChineseIDCardController> logger;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="personManager"></param>
    /// <param name="logger"></param>
    /// <param name="chineseIDCardManager"></param>
    public ChineseIDCardController(NaturalPersonManager personManager, ILogger<ChineseIDCardController> logger, ChineseIDCardManager chineseIDCardManager)
    {
        this.personManager = personManager;
        this.logger = logger;
        this.chineseIDCardManager = chineseIDCardManager;
    }

    /// <summary>
    /// 获取有关自然人的实名信息。
    /// </summary>
    /// <param name="personId"></param>
    /// <returns></returns>
    [HttpGet("{personId}")]
    public async Task<ActionResult<ChineseIDCardModel>> GetChineseIDCardInfo(string personId)
    {
        this.logger.LogInformation("End user {EndUser} get a chinese ID card info.", this.User.Identity!.Name);
        this.logger.LogInformation("Incoming access token is {accessToken}", await this.HttpContext.GetTokenAsync(JwtBearerDefaults.AuthenticationScheme, "access_token"));
        var person = await this.personManager.FindByIdAsync(personId);
        if (person == null)
        {
            return this.NotFound();
        }
        var currentCard = await this.chineseIDCardManager.GetCurrentAsync(person);
        return currentCard == null
            ? (ActionResult<ChineseIDCardModel>)this.NotFound()
            : (ActionResult<ChineseIDCardModel>)new ChineseIDCardModel(currentCard.ChineseIDCard.CardNumber,
                                      currentCard.ChineseIDCard.Name,
                                      currentCard.ChineseIDCard.Sex.ToString(),
                                      currentCard.ChineseIDCard.DateOfBirth,
                                      currentCard.ChineseIDCard.Ethnicity,
                                      currentCard.ChineseIDCard.Address,
                                      currentCard.ChineseIDCard.Issuer,
                                      currentCard.ChineseIDCard.IssueDate,
                                      currentCard.ChineseIDCard.Expires);
    }
}
