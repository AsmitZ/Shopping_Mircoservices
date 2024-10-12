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
}