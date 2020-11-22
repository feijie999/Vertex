using System;
using AutoMapper;
using Vertex.Crud.Test.Biz.Snapshot;
using Vertex.Grain.EntityFramework;
using Vertex.Runtime.Snapshot;
using Vertex.Runtime.Test.Events;

namespace Vertex.Runtime.Test.Snapshot
{
    public class AccountSnapshotHandler : CrudHandler<long, AccountSnapshot>
    {
        public void EventHandle(AccountSnapshot state, TopupEvent evt)
        {
            state.Balance = evt.Balance;
        }

        public void EventHandle(AccountSnapshot state, TransferArrivedEvent evt)
        {
            state.Balance = evt.Balance;
        }

        public void EventHandle(AccountSnapshot state, TransferEvent evt)
        {
            state.Balance = evt.Balance;
        }

        public void EventHandle(AccountSnapshot state, TransferRefundsEvent evt)
        {
            state.Balance = evt.Balance;
        }

        public void EventHandle(AccountSnapshot state, ErrorTestEvent evt)
        {
            throw new ArgumentException();
        }

        public AccountSnapshotHandler(IMapper mapper) : base(mapper)
        {
        }
    }
}
