namespace Application.Exceptions
{
    public class UserNotFoundException : ApplicationException
    {
        public UserNotFoundException(int userId) { this.userId = userId; }
        public int userId { get; set; }
    }
    public class CursorInvalidException : ApplicationException { }
    public class IdempotencyMismatchException : ApplicationException { }
    public class NotEnoughBalanceException : ApplicationException { }
}
