using MassTransit;
using Play.Catalog.Contracts;
using Play.Common;
using Play.Inventory.Service.Entities;

namespace Play.Inventory.Service.Consumers
{
    public class CatalogItemModifiedConsumer : IConsumer<CatalogItemModified>
    {
        public readonly IRepository<CatalogItem> repository;
        public CatalogItemModifiedConsumer(IRepository<CatalogItem> repository)
        {
            this.repository = repository;
        }
        public async Task Consume(ConsumeContext<CatalogItemModified> context)
        {
            var message = context.Message;
            var item = await repository.GetAsync(message.ItemId);

            if (item != null)
            {
                item.Id = message.ItemId;
                item.Description = message.Description;
                item.Name = message.Name;
                await repository.UpdateAsync(item);
                return;

            }
            item = new CatalogItem
            {
                Id = message.ItemId,
                Description = message.Description,
                Name = message.Name,
            };
            await repository.CreateAsync(item);
        }
    }
}
