using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using AzureFunction.Products.DTO;
using AzureFunction.Products.Entity;
using AzureFunction.Products.Service;
using AzureFunction.Utility;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;

namespace AzureFunction.Products.Http
{
    public class ProductFunction(IProductService productService)
    {
        private readonly IProductService _productService = productService;
        [Function("GetProducts")]
        public async Task<IActionResult> GetProducts([HttpTrigger(AuthorizationLevel.Anonymous, "get")] HttpRequest req)
        {
            // Implementation for the product function
            try
            {
                var products = await _productService.GetProducts();
                return HttpHelper.GenerateResponse(products);
            }
            catch (System.Exception ex)
            {
                return HttpHelper.GenerateErrorResponse(ex.Message, StatusCodes.Status500InternalServerError);
            }
        }

        [Function("GetSubsciptionsByProduct")]
        public async Task<IActionResult> GetSubsciptionsByProduct([HttpTrigger(AuthorizationLevel.Anonymous, "get")] HttpRequest req)
        {
            try
            {
                var productName = req.Query["productName"];

                if (string.IsNullOrEmpty(productName))
                {
                    return HttpHelper.GenerateErrorResponse("Missing product name", StatusCodes.Status400BadRequest);
                }
                
                var subscriptions = await _productService.GetSubscriptionsByProduct(productName);
                return HttpHelper.GenerateResponse(subscriptions);
            }
            catch (System.Exception ex)
            {
                return HttpHelper.GenerateErrorResponse(ex.Message, StatusCodes.Status500InternalServerError);
            }
        }

        [Function("Subscribe")]
        public async Task<IActionResult> Subscribe([HttpTrigger(AuthorizationLevel.Anonymous, "post")] HttpRequest req)
        {
            try
            {
                var subscriptionRequest = await JsonSerializer.DeserializeAsync<SubscriptionRequest>(req.Body, new JsonSerializerOptions());
                if (subscriptionRequest is null)
                {
                    return HttpHelper.GenerateErrorResponse("Invalid request body", StatusCodes.Status400BadRequest);
                }
                
                var primaryKey = await _productService.Subscribe(subscriptionRequest.ProductId);
                return HttpHelper.GenerateResponse(primaryKey);
            }
            catch (System.Exception ex)
            {
                return HttpHelper.GenerateErrorResponse(ex.Message, StatusCodes.Status500InternalServerError);
            }
        }
    }
}