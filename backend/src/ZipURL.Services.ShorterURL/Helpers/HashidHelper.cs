using HashidsNet;

namespace ZipURL.Services.ShorterURL.Helpers
{
    public static class HashidHelper
    {
        // Salt
        private const string Salt = "Salt_Secret_2026_For_ZipURL_ShortURL_Service";

        // MinHashLength
        private const int MinHashLength = 6;

        private static readonly Hashids _hashids = new Hashids(Salt, MinHashLength);

        public static string Encode(int id)
        {
            return _hashids.Encode(id);
        }

        public static int Decode(string code)
        {
            var decoded = _hashids.Decode(code);
            return decoded.Length > 0 ? decoded[0] : 0;
        }
    }
}
