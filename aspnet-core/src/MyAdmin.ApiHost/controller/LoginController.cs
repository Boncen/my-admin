using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using MyAdmin.Core.Model.BuildIn;
using MyAdmin.Core.Mvc;
using MyAdmin.Core.Repository;
using MyAdmin.Core.Utilities;
using ILogger = MyAdmin.Core.Logger.ILogger;

namespace MyAdmin.ApiHost.Controller;

public class LoginController : MAController
{
    private readonly ILogger _logger;
    private readonly IRepository<MaUser,Guid> _repository;
    private readonly IRepository<MaTenant,Guid> _tenantRepository;
    public LoginController(ILogger logger, IRepository<MaUser,Guid> repository,IRepository<MaTenant,Guid> tenantRepository)
    {
        _logger = logger;
        _repository = repository;
        _tenantRepository = tenantRepository;
    }

    // todo 生成验证码
    
    public async Task Login([FromBody] LoginReq req, CancellationToken cancellationToken)
    {
        if (req.tenantId.HasValue)
        {
           var tenant = await _tenantRepository.GetByIdAsync(req.tenantId.Value, cancellationToken);
        }

        var hashPasswd = PasswordHelper.HashPassword(req.password);
        // _repository.FindByIdAsync()
        // _repository.InsertAsync(new MaUser()
        // {
        //     Account = req.account,
        //     Password = 
        // });
        return;
    }
}

public record LoginReq([Required]string account, [Required]string password, Guid? tenantId);