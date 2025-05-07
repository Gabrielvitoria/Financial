using Financial.Domain.Dtos;
using Financial.Infra.Repositories;
using Xunit;

namespace Financial.Infra.Tests.Repositories
{
    public class UserRepositoryTests
    {
        private readonly UserRepository _userRepository;

        public UserRepositoryTests()
        {
            _userRepository = new UserRepository(); // Não há dependências para injetar no construtor no momento
        }

        [Fact(DisplayName = "Get deve retornar o UserDto correto para credenciais válidas (master)")]
        public void Get_ValidMasterCredentials_ReturnsMasterUserDto()
        {
            // Arrange
            string username = "master";
            string password = "master";

            // Act
            var user = _userRepository.Get(username, password);

            // Assert
            Assert.NotNull(user);
            Assert.Equal("master", user.Username);
            Assert.Equal("master", user.Password);
            Assert.Equal("gerente", user.Role);
        }

        [Fact(DisplayName = "Get deve retornar o UserDto correto para credenciais válidas (basic)")]
        public void Get_ValidBasicCredentials_ReturnsBasicUserDto()
        {
            // Arrange
            string username = "basic";
            string password = "basic";

            // Act
            var user = _userRepository.Get(username, password);

            // Assert
            Assert.NotNull(user);
            Assert.Equal("basic", user.Username);
            Assert.Equal("basic", user.Password);
            Assert.Equal("usuario", user.Role);
        }

        [Fact(DisplayName = "Get deve retornar null para username inválido")]
        public void Get_InvalidUsername_ReturnsNull()
        {
            // Arrange
            string username = "invalid";
            string password = "master";

            // Act
            var user = _userRepository.Get(username, password);

            // Assert
            Assert.Null(user);
        }

        [Fact(DisplayName = "Get deve retornar null para password inválido")]
        public void Get_InvalidPassword_ReturnsNull()
        {
            // Arrange
            string username = "invalid";
            string password = "invalid";

            // Act
            var user = _userRepository.Get(username, password);

            // Assert
            Assert.Null(user);
        }

        [Fact(DisplayName = "Get deve retornar null para username e password inválidos")]
        public void Get_InvalidUsernameAndPassword_ReturnsNull()
        {
            // Arrange
            string username = "invalid";
            string password = "invalid";

            // Act
            var user = _userRepository.Get(username, password);

            // Assert
            Assert.Null(user);
        }

        [Fact(DisplayName = "Get deve ser case-insensitive para o username")]
        public void Get_CaseInsensitiveUsername_ReturnsUserDto()
        {
            // Arrange
            string username = "MASTER";
            string password = "master";

            // Act
            var user = _userRepository.Get(username, password);

            // Assert
            Assert.NotNull(user);
            Assert.Equal("master", user.Username.ToLower()); // Verificando o username em minúsculo
        }
    }
}