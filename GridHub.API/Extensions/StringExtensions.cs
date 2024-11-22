namespace ERP_InsightWise.API.Extensions
{
    public static class StringExtensions
    {
        public static string ToFIAP(this string str)
        {
            return $"{str}@fiap.com.br";
        }
    }
}
