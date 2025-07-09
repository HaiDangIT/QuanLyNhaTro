namespace DACS2.Models.Order
{
    public class Order
    {
        public string OrderId { get; set; }
        public string UserId { get; set; } // nếu có thông tin user
        public string CourseId { get; set; } // nếu có thông tin khóa học
        public double Amount { get; set; }
        public string Status { get; set; } // Pending, Success, Failed
        public string? TransactionId { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime? PaymentDate { get; set; }
    }
}
