using System.Security.Cryptography;
using System.Text;

#nullable enable

namespace PeerAMid.Support;

public static class CryptoHelper
{
    private const string _defaultKey = "$Y38Te1x109$pbf6f81c41c#";
    private static readonly byte[] _defaultVector = { 59, 240, 3, 173, 29, 0, 76, 173 };

    public static string Encrypt(string plainText)
        => Encrypt(plainText, _defaultKey, _defaultVector);

    public static string Encrypt(string plainText, string encryptionKey, byte[] vector)
    {
        var desProvider = new TripleDESCryptoServiceProvider();
        var md5Provider = new MD5CryptoServiceProvider();

        try
        {
            //Debug.WriteLine("Encrypting " + plainText);
            var plainBytes = Encoding.UTF8.GetBytes(plainText);
            //Debug.WriteLine(plainBytes.Length);
            //Debug.WriteLine(plainBytes[0]);
            desProvider.Key = md5Provider.ComputeHash(Encoding.UTF8.GetBytes(encryptionKey));
            desProvider.IV = vector;
            var encrypted = desProvider.CreateEncryptor().TransformFinalBlock(plainBytes, 0, plainBytes.Length);
            //Debug.WriteLine(encrypted.Length);
            //Debug.WriteLine(encrypted[0]);
            var pretty = Convert.ToBase64String(encrypted);
            //Debug.WriteLine(pretty);
            return pretty;
        }
        catch (CryptographicException ex)
        {
            Log.Error(ex);
            throw;
        }
        catch (FormatException ex)
        {
            Log.Error(ex);
            throw;
        }
        catch (Exception ex)
        {
            Log.Error(ex);
            throw;
        }
        finally
        {
            desProvider.Clear();
            md5Provider.Clear();
        }
    }

    public static string Decrypt(string plainText)
         => Decrypt(plainText, _defaultKey, _defaultVector);

    public static string Decrypt(string encrypted, string encryptionKey, byte[] vector)
    {
        var desProvider = new TripleDESCryptoServiceProvider();
        var md5Provider = new MD5CryptoServiceProvider();

        try
        {
            //Debug.WriteLine("Decrypting " + encrypted);
            var encryptedBytes = Convert.FromBase64String(encrypted);
            //Debug.WriteLine(encrypted.Length);
            //Debug.WriteLine(encrypted[0]);
            desProvider.Key = md5Provider.ComputeHash(Encoding.UTF8.GetBytes(encryptionKey));
            desProvider.IV = vector;
            var decrypted = desProvider.CreateDecryptor().TransformFinalBlock(encryptedBytes, 0, encryptedBytes.Length);
            //Debug.WriteLine(decrypted.Length);
            //Debug.WriteLine(decrypted[0]);
            var plainText = Encoding.UTF8.GetString(decrypted);
            //Debug.WriteLine(plainText);
            return plainText;
        }
        catch (CryptographicException ex)
        {
            Log.Error(ex);
            throw;
        }
        catch (FormatException ex)
        {
            Log.Error(ex);
            throw;
        }
        catch (Exception ex)
        {
            Log.Error(ex);
            throw;
        }
        finally
        {
            desProvider.Clear();
            md5Provider.Clear();
        }
    }

    /// <summary>
    /// This routine extends Decrypt() by removing the '#' prefix from the encrypted string
    /// (if present) and by appending '=' characters to the end of the string to make it a 
    /// valid Base64 string (if needed).  After decryption, the routine a given prefix, if
    /// it is present.
    /// The prefix is used when we might have the same password encrypted several times,
    /// to obscure the fact that the same password is being used.  Its use is optional.
    /// </summary>
    /// <param name="encrypted"></param>
    /// <param name="possiblePrefix"></param>
    /// <returns></returns>
    public static string DecryptEx(string encrypted, string? possiblePrefix = null)
    {
        if (encrypted[0] == '#')
            encrypted = encrypted.Substring(1);

        // It's in base64, but we allow any terminating equals signs '='
        // to be omitted.  This might obscure things a bit.
        encrypted = (encrypted.Length % 4) switch
        {
            1 => encrypted + "===",
            2 => encrypted + "==",
            3 => encrypted + "=",
            _ => encrypted,
        };

        // Decrypt it
        var plain = Decrypt(encrypted);

        // We allow the password to be encrypted with a prefix on it, which is 
        // removed if present.
        if (!string.IsNullOrEmpty(possiblePrefix) && plain.StartsWith(possiblePrefix!))
            plain = plain.Substring(possiblePrefix!.Length);

        return plain;
    }
}
