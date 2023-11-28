using IdSubjects.RealName;

namespace AdminWebApp.Areas.RealName.Pages
{
    public class AuthenticationsModel : PageModel
    {
        private RealNameManager realNameManager;

        public AuthenticationsModel(RealNameManager realNameManager)
        {
            this.realNameManager = realNameManager;
        }

        public IEnumerable<RealNameAuthentication> Authentications { get; set; } = Enumerable.Empty<RealNameAuthentication>();

        public int Count { get; set; }

        public void OnGet()
        {
            IQueryable<RealNameAuthentication> set = this.realNameManager.Authentications.OrderByDescending(a => a.ValidatedAt);
            //应用过滤

            //计算结果数
            this.Count = set.Count();

            //应用分页


            this.Authentications = set.Take(1000).AsEnumerable();
        }
    }
}
