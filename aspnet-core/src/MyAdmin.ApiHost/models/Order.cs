using System.ComponentModel.DataAnnotations.Schema;
using MyAdmin.Core.Entity;
using MyAdmin.Core.Entity.Auditing;

namespace MyAdmin.ApiHost.models;

[Table("Order")]
public class Order:FullAuditedEntity<Guid>,IEntity<Guid>,ITenantObject<Guid>
{
    public Guid Id { get; set; }
    public string OrderNo { get; set; }
    public decimal Amount { get; set; }
    /// <summary>
    /// 订单商品概述
    /// </summary>
    public string DescBody { get; set; }
    public Guid TenantId { get; set; }
    
}