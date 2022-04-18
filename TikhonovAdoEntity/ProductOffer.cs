using System;
using System.Collections.Generic;

namespace TikhonovAdoEntity.Context
{
    public partial class ProductOffer
    {
        public double Price { get; set; }
        public string Name { get; set; } = null!;
        public int? InStock { get; set; }
        public int Category { get; set; }
        public int Id { get; set; }

        public virtual CategoryTable CategoryNavigation { get; set; } = null!;
    }
}
