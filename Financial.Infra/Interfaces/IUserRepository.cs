using Financial.Domain.Dtos;

namespace Financial.Infra.Interfaces
{
    public interface IUserRepository
    {
        UserDto Get(string username, string password);
    }
}
