using Core.Entities;
using Core.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    public class ShoppingCartController : BaseApiController
    {
        private readonly IShoppingCartService _shoppingCartService;

        public ShoppingCartController(IShoppingCartService shoppingCartService)
        {
            _shoppingCartService = shoppingCartService;
        }

        [HttpGet]
        public async Task<ActionResult<ShoppingCart>> GetShoppingCart(string id)
        {
            var shoppingCart = await _shoppingCartService.GetShoppingCartAsync(id);

            var result = shoppingCart ?? new ShoppingCart { Id = id };

            return Ok(result);
        }

        [HttpPost]
        public async Task<ActionResult<ShoppingCart>> UpdateShoppingCart(ShoppingCart shoppingCart)
        {
            var updateShoppingCart = await _shoppingCartService.SetShoppingCartAsync(shoppingCart);

            if (updateShoppingCart is null)
            {
                return BadRequest("Problem updating the shopping cart");
            }

            return Ok(updateShoppingCart);
        }

        [HttpDelete]
        public async Task<ActionResult> DeleteShoppingCart(string id)
        {
            var deleted = await _shoppingCartService.DeleteShoppingCartAsync(id);

            if (!deleted)
            {
                return BadRequest("Problem deleting the shopping cart");
            }

            return Ok();
        }
    }
}
