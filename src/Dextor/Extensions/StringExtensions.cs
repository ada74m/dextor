namespace Dextor.Extensions
{
    public static class StringExtensions
    {
        public static byte[] ToUtf8EncodedBytes(this string source)
        {
            return System.Text.Encoding.UTF8.GetBytes(source);
        }     
    }
}