using Tangy_Models;

using TangyWeb_Client.ViewModel;

namespace TangyWeb_Client.Service.IService;

public interface IPaymentService
{
    public Task<SuccessModelDTO> Checkout(StripePaymentDTO model);
}
