using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using MyAdmin.Core.Framework.Attribute;

namespace MyAdmin.Core.Model.BuildIn;

[BuiltIn]
[Table("RolePermission")]
public class RolePermission
{
    public Guid RoleId { get; set; }
    public Guid PermissionId { get; set; }
    public MaRole Role { get; set; }

    public MaPermission Permission { get; set; }
}