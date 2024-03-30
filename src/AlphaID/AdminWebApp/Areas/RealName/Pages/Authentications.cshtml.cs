using IdSubjects.RealName;

namespace AdminWebApp.Areas.RealName.Pages
{
    public class AuthenticationsModel(RealNameManager realNameManager) : PageModel
    {
        public IEnumerable<RealNameAuthentication> Authentications { get; set; } = [];

        public int Count { get; set; }

        public void OnGet()
        {
            IQueryable<RealNameAuthentication> set = realNameManager.Authentications.OrderByDescending(a => a.ValidatedAt);
            //应用过滤

            //计算结果数
            Count = set.Count();

            //应用分页


            Authentications = set.Take(1000).AsEnumerable();
        }
    }
}
