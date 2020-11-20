using Vertex.Abstractions.Serialization;
using Vertex.Crud.Test.Biz.Repository;
using Vertex.Transaction.Abstractions.Snapshot;

namespace Vertex.Crud.Test.Biz.Snapshot
{
    public class AccountSnapshot : AccountEntity, ITxSnapshot<AccountSnapshot>
    {
        public AccountSnapshot Clone(ISerializer serializer)
        {
            return new AccountSnapshot
            {
                Balance = this.Balance,
                Id = this.Id
            };
        }
    }
}
