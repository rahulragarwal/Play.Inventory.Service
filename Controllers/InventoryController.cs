using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Play.Common;
using Play.Inventory.Service.Dtos;
using Play.Inventory.Service.Entities;

namespace Play.Inventory.Service.Controllers
{
    [ApiController]
    [Route("inventory")]
    [Authorize()]
    public class InventoryController : ControllerBase
    {
        private readonly IRepository<InventoryItem> inventoryRepository;

        private readonly IRepository<CatalogItem> catalogItemRepository;



        public InventoryController(IRepository<InventoryItem> itemsRepository, IRepository<CatalogItem> catalogItemRepository)
        {
            this.inventoryRepository = itemsRepository;
            this.catalogItemRepository = catalogItemRepository;
        }
        [HttpGet]
        public async Task<ActionResult<IEnumerable<InventoryItemDto>>> GetAsync(Guid userId)
        {
            if (userId == Guid.Empty)
                return BadRequest();


            var inventoryItems = await inventoryRepository.GetAllAsync(item => item.UserId == userId);
            var inventoryItemsId = inventoryItems.Select(item => item.CatalogItemId);
            var catalogItems = await catalogItemRepository.GetAllAsync(catalogItem => inventoryItemsId.Contains(catalogItem.Id));

            var inventoryItemDtos = inventoryItems.Select(inventoryItem =>
            {
                var catalogItem = catalogItems.Single(catalogItem => catalogItem.Id == inventoryItem.CatalogItemId);
                return inventoryItem.AsDto(catalogItem.Name, catalogItem.Description);
            });

            return Ok(inventoryItemDtos);
        }
        [HttpPost]
        public async Task<ActionResult> PostAsync(GrantItemsDto grantItemDto)
        {
            var item = await inventoryRepository.GetAsync(
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
                await inventoryRepository.CreateAsync(newInventoryItem);
            }
            else
            {
                item.Quantity += grantItemDto.Quantity;
                await inventoryRepository.UpdateAsync(item);
            }
            return Ok();
        }

    }
}