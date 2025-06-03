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
        protected async Task<ActionResult> CreatePagedResult<T>(
            IGenericRepository<T> genericRepository,
            ISpecification<T> spec,
            int pageIndex,
            int pageSize) where T : BaseEntity
        {
            var items = await genericRepository.GetEntitiesWithSpecAsync(spec);

            var totalCount = await genericRepository.CountAsync(spec);

            ////var itemsTask = genericRepository.GetEntitiesWithSpecAsync(spec);

            ////var totalCountTask = genericRepository.CountAsync(spec);

            ////await Task.WhenAll(itemsTask, totalCountTask);

            ////var items = await itemsTask;

            ////var totalCount = await totalCountTask;

            ////Error: A second operation was started on this context instance before a previous operation completed. This is usually caused by different threads concurrently using the same instance of DbContext. For more information on how to avoid threading issues with DbContext, see https://go.microsoft.com/fwlink/?linkid=2097913.

            var pagination = new Pagination<T>(pageIndex, pageSize, totalCount, items);

            return Ok(pagination);
        }

        protected async Task<ActionResult> CreatePagedResult<T, TDto>(
            IGenericRepository<T> genericRepository,
            ISpecification<T> spec,
            int pageIndex,
            int pageSize,
            Func<T, TDto> toDto) where T : BaseEntity, IDtoConvertible
        {
            var items = await genericRepository.GetEntitiesWithSpecAsync(spec);

            var totalCount = await genericRepository.CountAsync(spec);

            var dtoItems = items.Select(toDto).ToList();

            var pagination = new Pagination<TDto>(pageIndex, pageSize, totalCount, dtoItems);

            return Ok(pagination);
        }
    }
}
