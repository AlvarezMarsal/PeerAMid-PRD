using PeerAMid.Support;

namespace CryptoTester;

internal class Program
{
    static void Main(string[] args)
    {
        while (true)
        {
            Console.Write("Text: ");
            var line = Console.ReadLine();
            if (string.IsNullOrEmpty(line))
                break;

            if (line.EndsWith('='))
            {
                try
                {
                    var decrypted = CryptoHelper.Decrypt(line ?? "");
                    Console.WriteLine("Decrypted: " + decrypted);
                }
                catch
                {
                    Console.WriteLine("Decrypted: " + "Could not be decrypted");
                }            
            }
            else
            {
                try
                {
                    var encrypted = CryptoHelper.Encrypt(line ?? "");
                    Console.WriteLine("Encrypted: " + encrypted);
                }
                catch
                {
                    Console.WriteLine("Encrypted: " + "Could not be encrypted");
                }
            }
        }
    }
}
