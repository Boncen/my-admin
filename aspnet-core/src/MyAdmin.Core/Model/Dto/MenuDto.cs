using MyAdmin.Core.Entity.Auditing;
using MyAdmin.Core.Framework.Attribute;
using MyAdmin.Core.Model.BuildIn;

namespace MyAdmin.Core.Model.Dto;

public class MenuDto
{
    public Guid? TenantId { get; set; }
    public Guid Id { get; set; }

    public Guid? ParentId { get; set; }
    public required string Name { get; set; }
    public string? Path { get; set; }

    public string? Icon { get; set; }
    public int Order { get; set; } = 0;
    public MenuType MenuType { get; set; } = MenuType.Page;
    public string? Label { get; set; }
    public string? Locale { get; set; }
    public List<MenuDto>? Children { get; set; }

}

public class AddMenuDto
{
    /// <summary>
    /// 父级菜单
    /// </summary>
    public Guid? ParentId { get; set; }
    public required string Name { get; set; }
    public string? Icon { get; set; }
    public string? Label { get; set; }
    public string? Locale { get; set; }
    /// <summary>
    /// 外链地址
    /// </summary>
    public string? Path { get; set; }
    /// <summary>
    /// 排序地址
    /// </summary>
    public int Order { get; set; } = 0;
    public MenuType MenuType { get; set; } = MenuType.Page;
}

public class MenuSearchDto : PageRequest
{
    public string? Name { get; set; }
    public string? Label { get; set; }
    public MenuType MenuType { get; set; }
}