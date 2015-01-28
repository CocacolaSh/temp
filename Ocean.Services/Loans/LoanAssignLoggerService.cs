using Ocean.Core.Data;
using Ocean.Data;
using Ocean.Entity;

namespace Ocean.Services
{
    public class LoanAssignLoggerService : ServiceBase<LoanAssignLogger>, ILoanAssignLoggerService
    {
        public LoanAssignLoggerService(IRepository<LoanAssignLogger> loanAssignLoggerRepository, IDbContext context)
            : base(loanAssignLoggerRepository, context)
        {
        }
    }
}