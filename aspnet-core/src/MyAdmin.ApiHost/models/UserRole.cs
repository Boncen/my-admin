using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyAdmin.ApiHost.models;

[Table("UserRole")]
public class UserRole
{
    [Key]
    public Guid UserId { get; set; }
    [Key]
    public Guid RoleId { get; set; }
}