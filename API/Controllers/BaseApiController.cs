using API.RequestHelper;
using Core.Entities;
using Core.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BaseApiController : ControllerBase
    {
        protected async Task<ActionResult> CreatePagedResult<T>(IGenericRepository<T> genericRepository, ISpecification<T> spec, int pageIndex, int pageSize) where T : BaseEntity
        {
            var items = await genericRepository.GetEntitiesWithSpecAsync(spec);

            var totalCount = await genericRepository.CountAsync(spec);

            var pagination = new Pagination<T>(pageIndex, pageSize, totalCount, items);

            return Ok(pagination);
        }
    }
}
