using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using MyAdmin.Core.Entity;
using MyAdmin.Core.Framework.Attribute;

namespace MyAdmin.Core.Model.BuildIn;

[BuiltIn]
[Table("UserRole")]
public class UserRole:IEntity
{
    public Guid UserId { get; set; }
    public Guid RoleId { get; set; }
    // public MaUser User { get; set; }
    //
    // public MaRole Role { get; set; }
}