using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RadiusCore
{
    public class ConnectionRequestPolicy
    {
        public string Name { get; set; } = null!;

        public int Order { get; set; }

        public string? ConnectMethod { get; set; }

        public virtual ICollection<RadiusCondition> Conditions { get; set; } = [];

        /// <summary>
        /// 改写网络策略身份验证设置。为null表示不改写。
        /// </summary>
        public string? AuthenticationMethod { get; set; }

        /// <summary>
        /// 转发连接请求/身份验证。指示是在此服务器上对请求进行身份验证，还是将请求转发到其他服务器。
        /// </summary>
        public string? RouteAuthentication { get; set; }

        /// <summary>
        /// 转发连接请求/记账。指示是在此服务器上记账，还是将请求转发到其他服务器。
        /// </summary>
        public string? RouteAccounting { get; set; }

        /// <summary>
        /// 选择将应用下列规则的属性。并按列表中的规则显示顺序处理规则。
        /// </summary>
        public string? AttributeReplace { get; set; }

        /// <summary>
        /// 将要发送到RADIUS客户端的属性。
        /// </summary>
        public string? ResponseAdditionalAttributes { get; set; }

        /// <summary>
        /// 将要发送到RADIUS客户端的供应商特定属性。
        /// </summary>
        public string? ResponseAdditionalVendorSpec { get; set; }
    }

    /// <summary>
    /// 条件。
    /// </summary>
    public abstract class RadiusCondition
    {
        /// <summary>
        /// 测试条件是否为真。
        /// </summary>
        /// <returns></returns>
        public abstract bool TestCondition(RadiusContext context);
    }
}
