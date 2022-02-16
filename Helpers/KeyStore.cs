namespace KiddieParadies.Helpers
{
    public class KeyStore
    {
        public string ArabicKey { get; set; }

        public string EnglishKey { get; set; }

        public KeyStore(string englishKey, string arabicKey)
        {
            EnglishKey = englishKey;
            ArabicKey = arabicKey;
        }

        public KeyStore()
        {
        }
    }
}