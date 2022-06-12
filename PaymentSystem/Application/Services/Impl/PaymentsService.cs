using Application.Exceptions;

namespace Application.Services.Impl
{
    public class PaymentsService : IPaymentsService
    {
        private DAL.Context _context;
        private IUserService _userService;

        public PaymentsService(DAL.Context context, IUserService userService)
        {
            _context = context;
            _userService = userService;
        }

        public DTO.PaymentsServiceDTOs getUserPayments(DTO.PaymentsRequest request)
        {
            if (!_userService.doesUserExist(request.id))
                throw new UserNotFoundException(request.id);

            if (request.cursor == null)
            {
                request.cursor = int.MaxValue;
            }
            else
            {
                // verifying cursor
                var cursorPayment = _context.Payments.Find(request.cursor);
                if (cursorPayment == null || (cursorPayment.FromId != request.id && cursorPayment.ToId != request.id))
                    throw new CursorInvalidException();
            }

            // getting payments
            var payments = _context.Payments.Where(p => p.Id < request.cursor && (p.FromId == request.id || p.ToId == request.id))
                .OrderByDescending(p => p.Id)
                .Take((int)request.limit)
                .Select(p => new DTO.Payment()
                {
                    id = p.Id,
                    amount = (uint)p.Amount,
                    date = p.Date,
                    description = p.Description,
                    isPaid = p.FromId == request.id,
                    userId = (p.FromId == request.id ? p.ToId : p.FromId) ?? 0
                });

            // new cursor
            int? cursor = null;
            if (payments.Count() > 0 && payments.Count() == request.limit)
                cursor = payments.Last().id;

            return new DTO.PaymentsServiceDTOs() { operations = payments.ToList(), cursor = cursor };
        }

        public void createPayment(DTO.CreatePaymentRequest request)
        {
            if (!_userService.doesUserExist(request.toId))
                throw new UserNotFoundException(request.toId);
            if (!_userService.doesUserExist(request.fromId))
                throw new UserNotFoundException(request.fromId);

            using (var transaction = _context.Database.BeginTransaction())
            {
                if (Utilities.Helpers.CheckIdempotency(_context, request.fromId, request.toId, request.amount, request.description, request.idempotencyKey))
                    return;

                // Checking if payment can be done          
                var user = _userService.getUserInfo(request.fromId);
                if (user.balance < request.amount)
                    throw new NotEnoughBalanceException();

                //Performing operation
                var newPayment = new DAL.Payment()
                {
                    Amount = (int)request.amount,
                    Description = request.description,
                    FromId = request.fromId,
                    ToId = request.toId,
                    IdempotencyKey = request.idempotencyKey
                };
                _context.Payments.Add(newPayment);
                _context.SaveChanges();
                transaction.Commit();
            }
        }
    }
}
