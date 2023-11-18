using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IdSubjects.SecurityAuditing.Events;

/// <summary>
/// 表示一个登录成功事件。
/// </summary>
public class CreatePersonSuccessEvent : AuditLogEvent
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="message"></param>
    public CreatePersonSuccessEvent(string? message = null) 
        : base(AuditLogEventCategories.AccountManagement, EventIds.CreatePersonSuccess, AuditLogEventTypes.Success, message)
    {
    }
}
