namespace Mango.Services.OrderAPI.Utility;

public static class SD
{
    public static class Status
    {
        public const string Pending = "Pending";
        public const string Approved = "Approved";
        public const string InProcess = "InProcess";
        public const string Shipped = "Shipped";
        public const string Canceled = "Canceled";
        public const string Refunded = "Refunded";
    }

    public static class Roles
    {
        public const string Admin = "Admin";
        public const string Customer = "Customer";
    }
}