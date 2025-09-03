using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace SP.WebApp.Controllers
{
    [Authorize(Roles = "Manager")]
    public class OrderDetailController : Controller
    {
        private const string ApiUrl = "https://localhost:7131/api/orderdetail";
        private readonly HttpClient _httpClient;

        public OrderDetailController(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient();
            _httpClient.Timeout = TimeSpan.FromSeconds(30);
        }

        public IActionResult Dashboard()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> GetAllStatistics()
        {
            try
            {
                // Thống kê cơ bản
                var pendingTask = _httpClient.GetFromJsonAsync<int>($"{ApiUrl}/products/total-pending");
                var shippingTask = _httpClient.GetFromJsonAsync<int>($"{ApiUrl}/products/total-shipping");
                var deliveredTask = _httpClient.GetFromJsonAsync<int>($"{ApiUrl}/products/total-delivered");
                var canceledTask = _httpClient.GetFromJsonAsync<int>($"{ApiUrl}/products/total-canceled");
                var totalRevenueTask = _httpClient.GetFromJsonAsync<decimal>($"{ApiUrl}/revenue/total");

                // Thống kê chi tiết
                var completedOrderTask = _httpClient.GetFromJsonAsync<int>($"{ApiUrl}/orders/completed-count");
                var totalOrdersTask = _httpClient.GetFromJsonAsync<int>($"{ApiUrl}/orders/total-count");
                var avgOrderValueTask = _httpClient.GetFromJsonAsync<decimal>($"{ApiUrl}/orders/avg-order-value");
                var topCustomersTask = _httpClient.GetFromJsonAsync<List<TopCustomer>>($"{ApiUrl}/customers/top-spending?count=5");
                var topProductsTask = _httpClient.GetFromJsonAsync<List<TopSellingProduct>>($"{ApiUrl}/products/top-selling?top=5");

                await Task.WhenAll(
                    pendingTask, shippingTask, deliveredTask, canceledTask, totalRevenueTask,
                    topCustomersTask, topProductsTask
                );

                var totalOrders = await totalOrdersTask;
                var completedOrders = await completedOrderTask;
                var canceledOrders = await canceledTask;

                var result = new
                {
                    BasicStats = new
                    {
                        PendingCount = await pendingTask,
                        ShippingCount = await shippingTask,
                        DeliveredCount = await deliveredTask,
                        CanceledCount = await canceledTask,
                        TotalRevenue = await totalRevenueTask
                    },
                    DetailedStats = new
                    {
                        CompletionRate = totalOrders > 0 ? Math.Round((completedOrders * 100.0) / totalOrders, 1) : 0,
                        CancellationRate = totalOrders > 0 ? Math.Round((canceledOrders * 100.0) / totalOrders, 1) : 0,
                        AverageOrderValue = await avgOrderValueTask,
                        TopCustomers = await topCustomersTask,
                        TopProducts = await topProductsTask
                    }
                };

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetRevenueByDateRange([FromQuery] DateTime from, [FromQuery] DateTime to)
        {
            try
            {
                if (from > to)
                {
                    return BadRequest("End date must be after start date");
                }

                var response = await _httpClient.GetFromJsonAsync<decimal>($"{ApiUrl}/revenue/total-by-range?from={from:yyyy-MM-dd}&to={to:yyyy-MM-dd}");
                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetRevenueByPeriod([FromQuery] string period = "month")
        {
            try
            {
                var response = await _httpClient.GetFromJsonAsync<RevenueData>($"{ApiUrl}/revenue/by-period?period={period}");
                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetTopProducts([FromQuery] int top = 10)
        {
            try
            {
                var response = await _httpClient.GetFromJsonAsync<IEnumerable<TopSellingProduct>>($"{ApiUrl}/products/top-selling?top={top}");
                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }

        public class TopSellingProduct
        {
            public int ProductVariantId { get; set; }
            public int Quantity { get; set; }
            public string Name { get; set; }
            public string Size { get; set; }
            public string Color { get; set; }
        }

        public class TopCustomer
        {
            public string Name { get; set; }
            public int OrderCount { get; set; }
            public decimal TotalSpent { get; set; }
        }

        public class RevenueData
        {
            public List<string> Labels { get; set; }
            public List<decimal> Values { get; set; }
        }
    }
}