namespace Application.Services
{
    public interface IPaymentsService
    {
        public DTO.PaymentsServiceDTOs getUserPayments(DTO.PaymentsRequest request);
        public void createPayment(DTO.CreatePaymentRequest request);
    }
}
