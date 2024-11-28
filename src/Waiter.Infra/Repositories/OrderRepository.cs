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
    }
}
