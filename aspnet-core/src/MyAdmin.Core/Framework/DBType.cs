using System.ComponentModel;

namespace MyAdmin.Core.Framework;

public enum DBType
{
    [Description("MySql")]
    MySql,
    [Description("MsSql")]
    MsSql,
    [Description("Postgre")]
    Postgre,
}