using AlphaIdPlatform;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AlphaIdWebAPI.Pages
{
    /// <summary>
    /// 
    /// </summary>
    /// <remarks>
    /// 
    /// </remarks>
    /// <param name="productionInfo"></param>
    public class IndexModel(IOptions<ProductInfo> productionInfo) : PageModel
    {

        /// <summary>
        /// 
        /// </summary>
        public ProductInfo ProductionInfo { get; } = productionInfo.Value;

        /// <summary>
        /// 
        /// </summary>
        public void OnGet()
        {
        }
    }
}
