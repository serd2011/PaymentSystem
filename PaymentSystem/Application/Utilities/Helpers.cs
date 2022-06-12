using Application.Exceptions;

namespace Application.Utilities
{
    internal static class Helpers
    {
        /// <summary> returns true if payment already exists and false otherwise </summary>
        /// <exception cref="IdempotencyMismatchException"/>
        public static bool CheckIdempotency(DAL.Context context, int? fromId, int? toId, uint amount, string description, string idempotencyKey)
        {
            var duplicatePayment = context.Payments.Where(p => p.IdempotencyKey == idempotencyKey && p.FromId == fromId).FirstOrDefault();
            if (duplicatePayment != null)
            {
                if (duplicatePayment.Amount != amount || duplicatePayment.ToId != toId || duplicatePayment.Description != description)
                    throw new IdempotencyMismatchException();
                return true;
            }
            return false;
        }
    }
}
