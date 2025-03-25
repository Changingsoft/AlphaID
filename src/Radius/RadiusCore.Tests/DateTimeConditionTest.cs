namespace RadiusCore.Tests;

public class DateTimeConditionTest()
{
    [Fact]
    public void TestMatrix()
    {
        var condition = new DateTimeCondition
        {
            // To set Wednesday and Thursday from 3:00 to 4:00 should be true.
            Matrix = [0x00, 0x00, 0x00, 0x18, 0x18, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00],
        };

        //时间日期条件没有使用RadiusContext，所以传入null.
        // Wednesday 3:00 should be true.
        condition.TimeProvider = new FrozenTimeProvider(new(2025, 1, 1, 3, 0, 0, TimeSpan.FromHours(8)));
        Assert.True(condition.TestCondition(default!));

        // Thursday 4:59 should be true.
        condition.TimeProvider = new FrozenTimeProvider(new(2025, 1, 2, 4, 59, 0, TimeSpan.FromHours(8)));
        Assert.True(condition.TestCondition(default!));

        // Thursday 2:59 should be false.
        condition.TimeProvider = new FrozenTimeProvider(new(2025, 1, 2, 2, 59, 0, TimeSpan.FromHours(8)));
        Assert.False(condition.TestCondition(default!));

        // Wednesday 5:00 should be false.
        condition.TimeProvider = new FrozenTimeProvider(new(2025, 1, 1, 5, 0, 0, TimeSpan.FromHours(8)));
        Assert.False(condition.TestCondition(default!));
    }
}
