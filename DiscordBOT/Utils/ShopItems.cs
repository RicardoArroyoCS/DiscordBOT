using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DiscordBOT.Utils
{
    public class ShopItems: List<ShopItem>
    {
        public void SortCostAscending()
        {
            this.OrderBy(i => i.Cost);
        }
    }
}
