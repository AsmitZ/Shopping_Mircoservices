namespace Mango.Web.Utilities;

public class SD
{
    public static string CouponApiBase { get; set; }
    public static string AuthApiBase { get; set; }
    public static string ProductApiBase { get; set; }
    public static string ShoppingCartApiBase { get; set; }
    public static string OrderApiBase { get; set; }
    public const string RoleAdmin = "ADMIN";
    public const string RoleCustomer = "CUSTOMER";
    public const string TokenCookie = "JwtToken";

    public enum ApiType
    {
        GET,
        POST,
        PUT,
        DELETE
    }

    public static class Status
    {
        public const string Pending = "Pending";
        public const string Approved = "Approved";
        public const string InProcess = "InProcess";
        public const string Shipped = "Shipped";
        public const string Canceled = "Canceled";
        public const string Refunded = "Refunded";
    }
}