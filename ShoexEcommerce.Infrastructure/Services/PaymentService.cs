using Microsoft.Extensions.Configuration;
using Newtonsoft.Json.Linq;
using Razorpay.Api;

public class PaymentService
{
    private readonly IConfiguration _config;

    public PaymentService(IConfiguration config)
    {
        _config = config;
    }

    public CreateOrderResponse CreateOrder(decimal amount)
    {
        var client = new RazorpayClient(
            _config["Razorpay:KeyId"],
            _config["Razorpay:KeySecret"]);

        Dictionary<string, object> options = new();

        options.Add("amount", (long)(amount * 100)); // Ensure amount is integer
        options.Add("currency", "INR");
        options.Add("receipt", Guid.NewGuid().ToString());

        Razorpay.Api.Order order = client.Order.Create(options);

        return new CreateOrderResponse
        {
            OrderId = order["id"].ToString(),
            Amount = amount,
            Key = _config["Razorpay:KeyId"]!
        };
    }

    public bool VerifySignature(string razorpayOrderId, string razorpayPaymentId, string signature)
    {
        try
        {
            string secret = _config["Razorpay:KeySecret"] ?? throw new ArgumentNullException("Razorpay:KeySecret is missing");
            string payload = razorpayOrderId + "|" + razorpayPaymentId;

            using (var hmac = new System.Security.Cryptography.HMACSHA256(System.Text.Encoding.UTF8.GetBytes(secret)))
            {
                var hash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(payload));
                string expectedSignature = BitConverter.ToString(hash).Replace("-", "").ToLower();

                return expectedSignature.Equals(signature?.Trim(), StringComparison.OrdinalIgnoreCase);
            }
        }
        catch
        {
            return false;
        }
    }
}