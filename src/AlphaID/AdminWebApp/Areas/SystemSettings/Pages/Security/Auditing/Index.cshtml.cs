using IdSubjects.SecurityAuditing;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AdminWebApp.Areas.SystemSettings.Pages.Security.Auditing
{
    public class IndexModel : PageModel
    {
        readonly AuditLogViewer auditLogViewer;

        public IndexModel(AuditLogViewer auditLogViewer)
        {
            this.auditLogViewer = auditLogViewer;
        }

        public int Count { get; set; }

        public IEnumerable<AuditLogEntry> Log { get; set; } = Enumerable.Empty<AuditLogEntry>();

        public void OnGet(int? s = null, int? l = null)
        {
            int skip = s ?? 0;
            int take = l ?? 1000;
            if(take > 1000)
                take = 1000;

            this.Count = this.auditLogViewer.Log.Count();
            this.Log = this.auditLogViewer.Log.Skip(skip).Take(take);
        }
    }
}
