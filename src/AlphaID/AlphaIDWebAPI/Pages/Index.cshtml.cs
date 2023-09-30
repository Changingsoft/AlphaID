using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AlphaIDWebAPI.Pages
{
    /// <summary>
    /// 
    /// </summary>
    public class IndexModel : PageModel
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="productionInfo"></param>
        public IndexModel(IOptions<ProductInfo> productionInfo)
        {
            this.ProductionInfo = productionInfo.Value;
        }

        /// <summary>
        /// 
        /// </summary>
        public ProductInfo ProductionInfo { get; }

        /// <summary>
        /// 
        /// </summary>
        public void OnGet()
        {
        }
    }
}
