using System;
using System.Collections.Generic;

namespace TikhonovAdoEntity.Context
{
    public partial class CategoryTable
    {
        public CategoryTable()
        {
            ProductOffers = new HashSet<ProductOffer>();
        }

        public int Category { get; set; }
        public string CategoryName { get; set; } = null!;

        public virtual ICollection<ProductOffer> ProductOffers { get; set; }
    }
}
