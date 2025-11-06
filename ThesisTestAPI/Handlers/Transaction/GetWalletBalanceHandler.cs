using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ThesisTestAPI.Entities;
using ThesisTestAPI.Models.Transaction;

namespace ThesisTestAPI.Handlers.Transaction
{
    public class GetWalletBalanceHandler: IRequestHandler<GetWalletRequest,(ProblemDetails?,long?)>
    {
        private readonly ThesisDbContext _db;
        public GetWalletBalanceHandler(ThesisDbContext db)
        {
            _db = db;
        }

        public async Task<(ProblemDetails?, long?)> Handle(GetWalletRequest request, CancellationToken cancellationToken)
        {
            var wallet = await _db.Wallets.Where(q => q.UserId == request.UserId).FirstOrDefaultAsync();
            return (null, wallet.BalanceMinor);
        }
    }
}
