using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

public static class Security
{
    private static readonly byte[] Salt = Encoding.ASCII.GetBytes("S0m3S@ltV@lu3"); // Substitua por um valor de sal apropriado
    private static readonly string EncryptionKey = "Minh@Chav3D3Cr1pt0graf14!Segura"; // Chave de criptografia segura

    public static string Criptografar(string strTexto)
    {
        if (string.IsNullOrEmpty(strTexto))
        {
            return string.Empty;
        }

        using (var aesAlg = Aes.Create())
        {
            using (var key = new Rfc2898DeriveBytes(EncryptionKey, Salt, 10000, HashAlgorithmName.SHA256))
            {
                aesAlg.Key = key.GetBytes(32);
                aesAlg.GenerateIV(); // Gera um IV exclusivo

                ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);

                using (var msEncrypt = new MemoryStream())
                {
                    // Armazena o IV no início do MemoryStream
                    msEncrypt.Write(aesAlg.IV, 0, aesAlg.IV.Length);

                    using (var csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                    {
                        using (var swEncrypt = new StreamWriter(csEncrypt))
                        {
                            swEncrypt.Write(strTexto);
                        }
                    }

                    return Convert.ToBase64String(msEncrypt.ToArray());
                }
            }
        }
    }

    public static string Descriptografar(string strTextoCriptografado)
    {
        if (string.IsNullOrEmpty(strTextoCriptografado))
        {
            return string.Empty;
        }

        byte[] fullCipher = Convert.FromBase64String(strTextoCriptografado);

        using (var aesAlg = Aes.Create())
        {
            using (var key = new Rfc2898DeriveBytes(EncryptionKey, Salt, 10000, HashAlgorithmName.SHA256))
            {
                aesAlg.Key = key.GetBytes(32);

                // Extrai o IV do início do texto criptografado
                byte[] iv = new byte[aesAlg.BlockSize / 8];
                Array.Copy(fullCipher, 0, iv, 0, iv.Length);

                aesAlg.IV = iv;

                ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);

                using (var msDecrypt = new MemoryStream(fullCipher, iv.Length, fullCipher.Length - iv.Length))
                {
                    using (var csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                    {
                        using (var srDecrypt = new StreamReader(csDecrypt))
                        {
                            return srDecrypt.ReadToEnd();
                        }
                    }
                }
            }
        }
    }
}
