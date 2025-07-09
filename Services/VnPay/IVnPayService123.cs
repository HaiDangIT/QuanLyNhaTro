using DACS2.Models.VnPay;

namespace DACS2.Services;
public interface IVnPayService123
{
    string CreatePaymentUrl(PaymentInformationModel model, HttpContext context);
    PaymentResponseModel PaymentExecute(IQueryCollection collections);

}