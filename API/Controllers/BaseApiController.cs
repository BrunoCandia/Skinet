using API.RequestHelper;
using Core.Entities;
using Core.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Threading;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BaseApiController : ControllerBase
    {
        protected async Task<ActionResult> CreatePagedResult<T>(
            IGenericRepository<T> genericRepository,
            ISpecification<T> spec,
            int pageIndex,
            int pageSize,
            CancellationToken cancellationToken = default) where T : BaseEntity
        {
            var items = await genericRepository.GetEntitiesWithSpecAsync(spec, cancellationToken);

            var totalCount = await genericRepository.CountAsync(spec, cancellationToken);

            var pagination = new Pagination<T>(pageIndex, pageSize, totalCount, items);

            return Ok(pagination);
        }

        protected async Task<ActionResult> CreatePagedResult<T, TDto>(
            IGenericRepository<T> genericRepository,
            ISpecification<T> spec,
            int pageIndex,
            int pageSize,
            Func<T, TDto> toDto,
            CancellationToken cancellationToken = default) where T : BaseEntity, IDtoConvertible
        {
            var items = await genericRepository.GetEntitiesWithSpecAsync(spec, cancellationToken);

            var totalCount = await genericRepository.CountAsync(spec, cancellationToken);

            var dtoItems = items.Select(toDto).ToList();

            var pagination = new Pagination<TDto>(pageIndex, pageSize, totalCount, dtoItems);

            return Ok(pagination);
        }
    }
}
