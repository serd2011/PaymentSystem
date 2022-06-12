using Application.Exceptions;

namespace Application.Services.Impl
{
    public class UserService : IUserService
    {
        private DAL.Context _context;

        public UserService(DAL.Context context)
        {
            _context = context;
        }

        public bool doesUserExist(int id)
        {
            return _context.Users.Find(id) != null;
        }

        public void createUser(int id)
        {
            var user = new DAL.User() { Id = id };
            _context.Users.Add(user);
            _context.SaveChanges();
        }

        public DTO.User getUserInfo(int id)
        {
            var user = _context.UsersBalances.Where(b => b.Id == id).FirstOrDefault();
            if (user == null)
                throw new UserNotFoundException(id);
            return new DTO.User() { id = user.Id, balance = (uint)user.Balance };
        }

        public void modifyBalance(DTO.ModifyBalanceRequest request)
        {
            if (!doesUserExist(request.id))
                throw new UserNotFoundException(request.id);

            using (var transaction = _context.Database.BeginTransaction())
            {
                int amount = Math.Abs(request.amount);
                int? fromId = request.amount > 0 ? null : request.id;
                int? toId = request.amount > 0 ? request.id : null;

                if (Utilities.Helpers.CheckIdempotency(_context, fromId, toId, (uint)amount, request.description, request.idempotencyKey))
                    return;

                // Checking if payment can be done
                if (getUserInfo(request.id).balance + request.amount < 0)
                    throw new NotEnoughBalanceException();

                //Performing operation
                var newPayment = new DAL.Payment()
                {
                    Amount = amount,
                    Description = request.description,
                    FromId = fromId,
                    ToId = toId,
                    IdempotencyKey = request.idempotencyKey
                };
                _context.Payments.Add(newPayment);
                _context.SaveChanges();
                transaction.Commit();
            }
        }
    }
}
