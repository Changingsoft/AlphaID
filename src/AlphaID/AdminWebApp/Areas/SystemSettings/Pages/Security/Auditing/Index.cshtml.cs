using IdSubjects.SecurityAuditing;

namespace AdminWebApp.Areas.SystemSettings.Pages.Security.Auditing
{
    public class IndexModel(AuditLogViewer auditLogViewer) : PageModel
    {
        public int Count { get; set; }

        public IEnumerable<AuditLogEntry> Log { get; set; } = [];

        public void OnGet(int? s = null, int? l = null)
        {
            int skip = s ?? 0;
            int take = l ?? 1000;
            if(take > 1000)
                take = 1000;

            Count = auditLogViewer.Log.Count();
            Log = auditLogViewer.Log.Skip(skip).Take(take);
        }
    }
}
