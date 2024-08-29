using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using MyAdmin.Core.Framework.Attribute;

namespace MyAdmin.Core.Model.BuildIn;

[BuiltIn]
[Table("RoleMenu")]
public class RoleMenu
{
    public Guid RoleId { get; set; }
    public Guid MenuId { get; set; }
    public MaRole Role { get; set; }

    public MaMenu Menu { get; set; }
}