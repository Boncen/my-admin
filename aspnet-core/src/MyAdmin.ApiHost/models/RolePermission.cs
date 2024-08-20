using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyAdmin.ApiHost.models;

[Table("RolePermission")]
public class RolePermission
{
    [Key]
    public Guid RoleId { get; set; }
    [Key]
    public Guid PermissionId { get; set; }

}