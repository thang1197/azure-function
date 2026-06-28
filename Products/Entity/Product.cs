using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AzureFunction.Products.Entity
{
    public class Product
    {
        public required string Id { get; set; }

        public required string Name { get; set; }

        public required string State { get; set; }

    }

    public class Subcription
    {
        public required string Id { get; set; }
        public required string Name { get; set; }

        public required string PrimaryKey { get; set; }

        public required string SecondaryKey { get; set; }
    }
}