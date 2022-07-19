using Microsoft.AspNetCore.Mvc;
using Play.Common;
using Play.Inventory.Service.Dtos;
using Play.Inventory.Service.Entities;

namespace Play.Inventory.Service.Controllers
{
    [ApiController]
    [Route("inventory")]
    public class InventoryController : ControllerBase
    {
        private readonly IRepository<InventoryItem> itemsRepository;
        public InventoryController(IRepository<InventoryItem> itemsRepository)
        {
            this.itemsRepository = itemsRepository;
        }
        [HttpGet]
        public async Task<ActionResult<IEnumerable<InventoryItem>>> GetAsync(Guid userId)
        {
            if (userId == Guid.Empty)
                return BadRequest();
            var items = (await itemsRepository.GetAllAsync(item => item.UserId == userId))
                        .Select(item => item.AsDto());
            return Ok(items);
        }
        [HttpPost]
        public async Task<ActionResult> PostAsync(GrantItemsDto grantItemDto)
        {
            var item = await itemsRepository.GetAsync(
                item => item.UserId == grantItemDto.UserId && item.CatalogItemId == grantItemDto.CatalogItemId);

            if (item == null)
            {
                var newInventoryItem = new InventoryItem
                {
                    UserId = grantItemDto.UserId,
                    Quantity = grantItemDto.Quantity,
                    CatalogItemId = grantItemDto.CatalogItemId,
                    AcquiredDate = DateTimeOffset.Now
                };
                await itemsRepository.CreateAsync(newInventoryItem);
            }
            else
            {
                item.Quantity += grantItemDto.Quantity;
                await itemsRepository.UpdateAsync(item);
            }
            return Ok();
        }

    }
}