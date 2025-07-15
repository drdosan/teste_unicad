namespace Raizen.UniCad.BLL.Extensions
{
    public static class StringExtensions
    {
        public static string RemoverZerosAEsquerda(this string source)
        {
            return source.TrimStart('0');
        }
    }
}
