using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AzureFunction.Products.Entity;

namespace AzureFunction.Products.Service
{
    public interface IProductService
    {
        public Task<List<Product>> GetProducts();

        public Task<List<Subcription>> GetSubscriptionsByProduct(string product);

        public Task<string> Subscribe(string product);

        public Task<string> Unsubscribe(string product);

        public Task<Product> GetProductByProductId(string product);
    }
}