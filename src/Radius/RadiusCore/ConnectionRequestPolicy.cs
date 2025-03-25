namespace RadiusCore
{
    /// <summary>
    /// 连接请求策略。
    /// </summary>
    public class ConnectionRequestPolicy
    {
        /// <summary>
        /// 策略名称。
        /// </summary>
        public string Name { get; set; } = null!;

        /// <summary>
        /// 应用顺序。
        /// </summary>
        public int Order { get; set; }

        /// <summary>
        /// 网络连接方法。
        /// 选择向NPS发送连接请求的网络访问服务器类型。你可以选择网络访问服务器的类型或特定于供应商的类型，也可以不选择。
        /// 如果你的网络访问服务器是802.1X身份验证交换机或无线访问点，请选择未指定。
        /// </summary>
        public string? ConnectionMethod { get; set; }

        /// <summary>
        /// 条件集合。
        /// </summary>
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

        /// <summary>
        /// 测试策略条件是否全部满足。若策略条件集合全部满足，则返回true，否则返回false。
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public virtual bool TestCondition(RadiusContext context)
        {
            return Conditions.All(condition => condition.TestCondition(context));
        }
    }
}
