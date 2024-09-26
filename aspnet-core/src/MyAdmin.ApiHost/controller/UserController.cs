using Mapster;
using Microsoft.AspNetCore.Mvc;
using MyAdmin.ApiHost.Db;
using MyAdmin.Core.Identity;
using MyAdmin.Core.Model;
using MyAdmin.Core.Model.BuildIn;
using MyAdmin.Core.Model.Dto;
using MyAdmin.Core.Mvc;
using MyAdmin.Core.Repository;

namespace MyAdmin.ApiHost.controller
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class UserController : MAController
    {
        private readonly ICurrentUser _currentUser;
        private readonly IRepository<MaUser,Guid, AdminTemplateDbContext> _repo;
        public UserController(ICurrentUser currentUser, IRepository<MaUser,Guid, AdminTemplateDbContext> repo)
        {
            _currentUser = currentUser;
            _repo = repo;
        }
        
        [HttpPost]
        public async Task<ApiResult<UserDto>> Info(CancellationToken cancellationToken){
            var currentUser = _currentUser.GetCurrentUser(true);
            var maUser = await _repo.GetByIdAsync(Guid.Parse(currentUser!.id),cancellationToken);
            return ApiResult<UserDto>.Ok((maUser as MaUser).Adapt<UserDto>());
        }
    }
}
