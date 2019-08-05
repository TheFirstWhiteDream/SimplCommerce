using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using SimplCommerce.Infrastructure.Data;
using SimplCommerce.Module.Core.Extensions;
using SimplCommerce.Module.Core.Services;
using SimplCommerce.Module.Orders.Models;
using SimplCommerce.Module.Orders.Services;
using SimplCommerce.Module.PaymentPaypalSmart.Services;
using SimplCommerce.Module.PaymentPaypalSmart.ViewModels;
using SimplCommerce.Module.Payments.Models;
using SimplCommerce.Module.ShoppingCart.Services;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Threading.Tasks;

namespace SimplCommerce.Module.PaymentPaypalSmart.Areas.PaymentPaypalSmart.Controllers
{
    [Area("PaymentPaypalSmart")]
    [ApiExplorerSettings(IgnoreApi = true)]
    public class PaypalSmartController : Controller
    {
        private readonly IRepository<Payment> _paymentRepository;
        private readonly ICurrencyService _currencyService;
        private readonly ICartService _cartService;
        private readonly IOrderService _orderService;
        private readonly IWorkContext _workContext;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IPaypalSmartService _paypalSmartService;

        public PaypalSmartController(
            IRepository<Payment> repository,
            ICurrencyService currencyService,
            ICartService cartService,
            IOrderService orderService,
            IWorkContext workContext,
            IHttpContextAccessor httpContextAccessor,
            IPaypalSmartService paypalSmartService
            )
        {
            _paymentRepository = repository;
            _currencyService = currencyService;
            _cartService = cartService;
            _orderService = orderService;
            _workContext = workContext;
            _httpContextAccessor = httpContextAccessor;
            _paypalSmartService = paypalSmartService;
        }

        [HttpPost("PaypalSmart/CreatePayment")]
        public async Task<IActionResult> CreatePayment()
        {
            var currentUser = await _workContext.GetCurrentUser();
            var cart = await _cartService.GetActiveCartDetails(currentUser.Id);
            if (cart == null)
            {
                return NotFound();
            }
            var regionInfo = new RegionInfo(_currencyService.CurrencyCulture.LCID);
            var paypalAcceptedNumericFormatCulture = CultureInfo.CreateSpecificCulture("en-US");
            var orderRequest = new OrderRequest()
            {
                intent = "CAPTURE",
                purchase_units = new List<PurchaseUnit>()
                {
                    new PurchaseUnit
                    {
                       amount = new Amount
                        {
                            currency_code = regionInfo.ISOCurrencySymbol,
                            value = cart.OrderTotal.ToString("N2", paypalAcceptedNumericFormatCulture)
                         }
                    }
                }
            };

            var response = await _paypalSmartService.CreatePayment(orderRequest, currentUser);
            var responseBody = await response.Content.ReadAsStringAsync();
            dynamic payment = JObject.Parse(responseBody);
            if(response.IsSuccessStatusCode)
            {
                var orderId = payment.id;
                return Ok(new { OrderID = orderId });
            }

            return BadRequest(responseBody);
        }

        [HttpPost("PaypalSmart/ExecutePayment/{orderID}")]
        public async Task<IActionResult> ExecutePayment(string orderID)
        {
            var currentUser = await _workContext.GetCurrentUser();
            var cart = await _cartService.GetActiveCartDetails(currentUser.Id);
            var orderCreateResult = await _orderService.CreateOrder(cart.Id, "PaypalSmart", cart.OrderTotal, OrderStatus.PendingPayment);
            if (!orderCreateResult.Success)
            {
                return BadRequest(orderCreateResult.Error);
            }
            var order = orderCreateResult.Value;
            var payment = new Payment()
            {
                OrderId = order.Id,
                PaymentFee = order.PaymentFeeAmount,
                Amount = order.OrderTotal,
                PaymentMethod = "Paypal Smart",
                CreatedOn = DateTimeOffset.UtcNow,
            };
            var response = await _paypalSmartService.ExecutePayment(orderID);
            var responseBody = await response.Content.ReadAsStringAsync();
            dynamic resObj = JObject.Parse(responseBody);
            if(response.IsSuccessStatusCode)
            {
                payment.GatewayTransactionId = resObj.id;
                payment.Status = PaymentStatus.Succeeded;
                _paymentRepository.Add(payment);
                order.OrderStatus = OrderStatus.PaymentReceived;
                await _paymentRepository.SaveChangesAsync();
                return Ok(new { Status = "success", OrderId = order.Id });
            }

            payment.Status = PaymentStatus.Failed;
            payment.FailureMessage = responseBody;
            _paymentRepository.Add(payment);
            order.OrderStatus = OrderStatus.PaymentFailed;
            await _paymentRepository.SaveChangesAsync();

            string errorName = resObj.name;
            string errorDescription = resObj.message;
            return BadRequest($"{errorName} - {errorDescription}");
        }
    }
}
