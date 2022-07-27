using Play.Inventory.Service.Dtos;
using Play.Inventory.Service.Entities;

namespace Play.Inventory.Service
{
    public static class Extensions
    {
        public static InventoryItemDto AsDto(this InventoryItem item, string Name, string Description )
        {
            return new InventoryItemDto(item.CatalogItemId, item.Quantity, Name, Description, item.AcquiredDate);
        }

    }

}