using Tangy_Models;

namespace TangyWeb_Client.Service.IService;

public interface IAuthenticationService
{
    public Task<SignUpResponseDTO> RegisterUser(SignUpRequestDTO signUpRequestDTO);
    public Task<SignInResponseDTO> Login(SignInRequestDTO signInRequestDTO);
    public Task Logout();
}
