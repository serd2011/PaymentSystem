namespace Application.Services
{
    public interface IUserService
    {
        public bool doesUserExist(int id);
        public void createUser(int id);
        public DTO.User getUserInfo(int id);
        public void modifyBalance(DTO.ModifyBalanceRequest request);
    }
}
