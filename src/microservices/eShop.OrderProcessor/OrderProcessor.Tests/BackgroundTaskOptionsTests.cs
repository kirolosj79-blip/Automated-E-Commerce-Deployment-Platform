namespace eShop.OrderProcessor.Tests;

public class BackgroundTaskOptionsTests
{
    [Fact]
    public void Default_instance_has_zero_values()
    {
        var options = new BackgroundTaskOptions();

        Assert.Equal(0, options.GracePeriodTime);
        Assert.Equal(0, options.CheckUpdateTime);
    }

    [Fact]
    public void Properties_round_trip()
    {
        var options = new BackgroundTaskOptions
        {
            GracePeriodTime = 30,
            CheckUpdateTime = 60,
        };

        Assert.Equal(30, options.GracePeriodTime);
        Assert.Equal(60, options.CheckUpdateTime);
    }
}
