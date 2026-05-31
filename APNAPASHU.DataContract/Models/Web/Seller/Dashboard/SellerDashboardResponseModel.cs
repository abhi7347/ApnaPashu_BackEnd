using System;
using System.Collections.Generic;

namespace APNAPASHU.DataContract.Models.Web.Seller.Dashboard
{
    public class SellerStatsModel
    {
        public int ActiveAnimalCount { get; set; }
        public int SoldAnimalCount { get; set; }
        public int TotalLeads { get; set; }
        public int TotalViews { get; set; }
    }

    public class RecentLeadModel
    {
        public int ConversationId { get; set; }
        public int AnimalId { get; set; }
        public string AnimalName { get; set; }
        public int BuyerId { get; set; }
        public string BuyerName { get; set; }
        public string LastMessage { get; set; }
        public DateTime? LastMessageDate { get; set; }
    }

    public class SellerDashboardResponseModel
    {
        public SellerStatsModel Stats { get; set; }
        public List<RecentLeadModel> RecentLeads { get; set; }

        public SellerDashboardResponseModel()
        {
            Stats = new SellerStatsModel();
            RecentLeads = new List<RecentLeadModel>();
        }
    }
}
