using DACS2.Models;
using DACS2.Models.Momo;
using DACS2.Models.Order;

namespace DACS2.Services.Momo
{
    public interface IMomoService
    {
        Task<MomoCreatePaymentResponseModel> CreatePaymentAsync(OrderInfoModel model);
        MomoExecuteResponseModel PaymentExecuteAsync(IQueryCollection collection);

    }
}
