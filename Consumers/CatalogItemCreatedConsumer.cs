using MassTransit;
using Play.Catalog.Contracts;
using Play.Common;
using Play.Inventory.Service.Entities;

namespace Play.Inventory.Service.Consumers
{
    public class CatalogItemCreatedConsumer : IConsumer<CatalogItemAdded>
    {
        public readonly IRepository<CatalogItem> repository;
        public CatalogItemCreatedConsumer(IRepository<CatalogItem> repository)
        {
            this.repository = repository;
        }

        public async Task Consume(ConsumeContext<CatalogItemAdded> context)
        {
            var message = context.Message;
            var item = await repository.GetAsync(message.ItemId);

            if (item != null) return;

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
