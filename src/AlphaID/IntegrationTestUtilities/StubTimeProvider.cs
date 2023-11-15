using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntegrationTestUtilities;

/// <summary>
/// 提供测试用时间。
/// </summary>
public class StubTimeProvider : TimeProvider
{
    private DateTimeOffset time;

    public StubTimeProvider(DateTimeOffset time)
    {
        this.time = time;
    }

    public override DateTimeOffset GetUtcNow()
    {
        return this.time.ToUniversalTime();
    }
}
