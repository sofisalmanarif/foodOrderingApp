using foodOrderingApp.interfaces;
using foodOrderingApp.Services;
using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace foodOrderingApp.obj
{
    public class CashfreePayment : IPayment
    {
        private readonly string _apiKey;
        private readonly string _secretKey;
        private readonly bool _isProduction;
        private readonly IUserRepository _userRepository;
        private readonly HttpClient _httpClient;
        private readonly string _baseUrl;

        public CashfreePayment(IConfiguration configuration, IUserRepository userRepository, IHttpClientFactory httpClientFactory)
        {
            _userRepository = userRepository;
            // Get credentials from configuration
            _apiKey = configuration["Cashfree:ApiKey"] ;
            _secretKey = configuration["Cashfree:SecretKey"];
            _isProduction = configuration.GetValue<bool>("Cashfree:IsProduction");

            // Set the base URL based on environment
            _baseUrl = _isProduction
                ? "https://api.cashfree.com/pg"
                : "https://sandbox.cashfree.com/pg";

            _httpClient = httpClientFactory.CreateClient("CashfreeClient");
            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            _httpClient.DefaultRequestHeaders.Add("x-api-version", "2022-09-01");
            _httpClient.DefaultRequestHeaders.Add("x-client-id", _apiKey);
            _httpClient.DefaultRequestHeaders.Add("x-client-secret", _secretKey);
        }

        public string CreateOrder(double amount, Guid userId)
        {
            var user = _userRepository.GetById(userId);

            // Create the order request payload
            var orderRequest = new
            {
                order_amount = amount,
                order_currency = "INR",
                order_note = "Some information about the order",
                customer_details = new
                {
                    customer_id = user.Id.ToString(),
                    customer_email = user.Email,
                    customer_phone = user.Phone
                },
               
            };

            try
            {
                // Convert to JSON
                var jsonContent = JsonConvert.SerializeObject(orderRequest);
                var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

                // Add request ID header
                _httpClient.DefaultRequestHeaders.Add("x-request-id", Guid.NewGuid().ToString());

                // Make the API call synchronously (you can make it async if needed)
                var response = _httpClient.PostAsync($"{_baseUrl}/orders", content).Result;

                if (response.IsSuccessStatusCode)
                {
                    var responseContent = response.Content.ReadAsStringAsync().Result;
                    var responseObject = JsonConvert.DeserializeObject<dynamic>(responseContent);

                    // Return the payment session ID
                    return responseObject.payment_session_id;
                }
                else
                {
                    var responseContent = response.Content.ReadAsStringAsync().Result;
                    Console.WriteLine($"Cashfree API Error: {responseContent}");
                    throw new AppException("Payment gateway error", HttpStatusCode.InternalServerError);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception in CreateOrder: {ex.Message}");
                throw new AppException("Something went wrong with payment processing", HttpStatusCode.InternalServerError);
            }
        }
    }
}