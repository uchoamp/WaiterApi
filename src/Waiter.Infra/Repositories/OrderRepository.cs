using System.Data;
using Microsoft.EntityFrameworkCore;
using Waiter.Application.Interfaces;
using Waiter.Domain.Models;
using Waiter.Domain.Repositories;
using Waiter.Infra.Data;

namespace Waiter.Infra.Repositories
{
    public class OrderRepository : BaseRepository<Order>, IOrderRepository
    {
        private readonly IUser _user;

        public OrderRepository(ApplicationDbContext dbContext, IUser user)
            : base(dbContext)
        {
            _user = user;
        }

        public override void Update(Order entity)
        {
            entity.UpdatedBy = _user.Id!.Value;
            base.Update(entity);
        }

        public override async Task RefreshAsync(Order entity)
        {
            await DbContext.Entry(entity).ReloadAsync();
            await DbContext.Entry(entity).Reference(x => x.Customer).LoadAsync();
            DbContext.Entry(entity).Collection(x => x.Items).IsLoaded = false;
            await DbContext.Entry(entity).Collection(x => x.Items).LoadAsync();
        }
    }
}
