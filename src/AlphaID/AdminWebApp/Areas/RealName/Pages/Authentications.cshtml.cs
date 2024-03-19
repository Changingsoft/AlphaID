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
            //Ӧ�ù���

            //��������
            this.Count = set.Count();

            //Ӧ�÷�ҳ


            this.Authentications = set.Take(1000).AsEnumerable();
        }
    }
}
