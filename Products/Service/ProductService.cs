using Azure.Core;
using Azure.ResourceManager;
using Azure.ResourceManager.ApiManagement;
using AzureFunction.Configuration.App;
using AzureFunction.Products.Entity;
using Microsoft.Extensions.Options;

namespace AzureFunction.Products.Service
{
    public class ProductService(ArmClient armClient, IOptions<ApiManagementSettings> apiManagementSettings) : IProductService
    {
        private readonly ArmClient _armClient = armClient;
        private readonly ApiManagementSettings _apiManagementSettings = apiManagementSettings.Value;
        public Task<Product> GetProductByProductId(string product)
        {
            throw new NotImplementedException();
        }

        public async Task<List<Product>> GetProducts()
        {
            try
            {
                var products = new List<Product>();
                var resoureIdentifier = ApiManagementServiceResource.CreateResourceIdentifier(
                    _apiManagementSettings.SubscriptionId,
                    _apiManagementSettings.ResourceGroupName,
                    _apiManagementSettings.ServiceName
                );
                var apiResource = _armClient.GetApiManagementServiceResource(resoureIdentifier);

                var apiProducts = apiResource.GetApiManagementProducts();

                await foreach (var item in apiProducts.GetAllAsync())
                {
                    var product = new Product
                    {
                        Id = item.Data.Id.ToString(),
                        Name = item.Data.Name,
                        State = item.Data.State?.ToString().ToLower() ?? "inactive",
                    };
                    products.Add(product);
                }

                return products;
            }
            catch (System.Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw new Exception("Error retrieving products: " + ex.Message);
            }
        }

        public async Task<List<Subcription>> GetSubscriptionsByProduct(string product)
        {
            try
            {
                var subscriptions = new List<Subcription>();
                var resourceIdentifier = ApiManagementProductResource.CreateResourceIdentifier(
                    _apiManagementSettings.SubscriptionId,
                    _apiManagementSettings.ResourceGroupName,
                    _apiManagementSettings.ServiceName,
                    product
                );

                var productResource = _armClient.GetApiManagementProductResource(resourceIdentifier);
                var subscriptionList = productResource.GetAllProductSubscriptionData();
                foreach (var item in subscriptionList)
                {
                    var resoureIdentifier = ApiManagementSubscriptionResource.CreateResourceIdentifier(
                        _apiManagementSettings.SubscriptionId,
                        _apiManagementSettings.ResourceGroupName,
                        _apiManagementSettings.ServiceName,
                        item.Name
                    );

                    var subscriptionResource = _armClient.GetApiManagementSubscriptionResource(resoureIdentifier);

                    var secrets = await subscriptionResource.GetSecretsAsync();

                    var subscription = new Subcription
                    {
                        Id = item.Id.ToString(),
                        Name = item.Name,
                        PrimaryKey = secrets.Value.PrimaryKey,
                        SecondaryKey = secrets.Value.SecondaryKey
                    };
                    subscriptions.Add(subscription);
                }
                return subscriptions;
            }
            catch (System.Exception ex)
            {

                throw new Exception("Error retrieving subscriptions: " + ex.Message);
            }
        }

        public async Task<string> Subscribe(string product)
        {
            try
            {
                var subscriptions = await GetSubscriptionsByProduct(product);
                if(subscriptions.Count > 0)
                {
                    return subscriptions[0].PrimaryKey;
                }
                
                throw new Exception("Cannot find product subscription");
            }
            catch (System.Exception ex)
            {
                
                throw new Exception("Error retrieving subscriptions: " + ex.Message);
            }
        }

        public Task<string> Unsubscribe(string product)
        {
            throw new NotImplementedException();
        }
    }
}