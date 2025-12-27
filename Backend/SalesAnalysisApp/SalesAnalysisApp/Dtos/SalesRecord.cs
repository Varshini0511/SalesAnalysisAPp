namespace SalesAnalysisApp.Dtos
{
    public class SalesRecord
    {
        public int OrderId {  get; set; }
        public int ProductId { get; set; }
        public int CustomerId { get; set; }
        public string RegionName { get; set; }
        public string CategoryName { get; set; }
        public DateTime DateOfSale { get; set; }
        public int QuantitySold { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal Discount  { get; set; }
        public decimal ShippingCOst { get; set; }
        public string PaymentMethodName { get; set; }
        public string CustomerName { get; set; }

         public string ProductName { get; set; }
    }
}
