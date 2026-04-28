namespace ZipURL.Services.ShorterURL.Helpers
{
    public class Base62Converter
    {
        private const string Charset = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";

        public static string Encode(int number)
        {
            if (number == 0) return Charset[0].ToString();

            string result = string.Empty;
            while (number > 0)
            {
                result = Charset[number % 62] + result;
                number /= 62;
            }
            return result;
        }

        public static int Decode(string base62Str)
        {
            int result = 0;
            foreach (var c in base62Str)
            {
                result = result * 62 + Charset.IndexOf(c);
            }
            return result;
        }
    }
}
