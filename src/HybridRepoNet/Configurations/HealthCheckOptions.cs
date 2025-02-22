using System.ComponentModel;

namespace HybridRepoNet.Configurations;

public enum HealthCheck
{
    [Description("Check Db connection")]
    Active,
    [Description("Do not check Db connection")]
    Inactive
}