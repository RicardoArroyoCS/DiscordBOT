using System;
using System.Collections.Generic;
using System.Text;

namespace DiscordBOT.Utils
{
    public class ShopItem
    {
        public ShopItem(string name, int cost, string description, bool isManual, int numberParamsRequired = 0)
        {
            Name = name;
            Cost = cost;
            Description = description;
            IsManualRequest = isManual;
            NumberParams = numberParamsRequired;
        }
        public string Name
        {
            get;
        }

        public int Cost
        {
            get;
        }

        public string Description
        {
            get;
        }

        public bool IsManualRequest
        {
            get;
        }

        public string ManualRequestDisplayText
        {
            get
            {
                return IsManualRequest ? "Manual" : "Automatic";
            }
        }

        public int NumberParams
        {
            get;
        }

        public override string ToString()
        {
            return String.Format(ShopManager._tableFormat, Name, Cost, Description, ManualRequestDisplayText);
        }
    }
}
