using Microsoft.AspNetCore.Mvc;
using Readery.Utilities.Interfaces;

namespace Readery.Web.Areas.Customer.Controllers
{
    public class PaymentController : Controller
    {
        private string PaypalClientId { get; set; } = string.Empty;
        private string PaypalSecret { get; set; } = string.Empty;
        private string PaypalUrl { get; set; } = string.Empty;

        private readonly PayPalService _payPalService;

        public PaymentController(PayPalService payPalService, IConfiguration configuration)
        {
            _payPalService = payPalService;

            // Initialize PayPal settings from configuration
            PaypalClientId = configuration["PaypalOptions:ClientId"];
            PaypalSecret = configuration["PaypalOptions:SecretId"];
            PaypalUrl = configuration["PaypalOptions:Url"];
        }

        public async Task<IActionResult> Index()
        {
            ViewBag.PaypalClientId = PaypalClientId;
            return View();
        }
    }
}
