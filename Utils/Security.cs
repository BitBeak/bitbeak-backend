using DotNetEnv;
using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

public class Security
{
    private static byte[] _salt;
    private static string _encryptionKey;

    static Security()
    {
        Env.Load();  // Carrega as variáveis de ambiente do arquivo .env

        // Acessando as variáveis de ambiente
        _salt = Encoding.ASCII.GetBytes(Environment.GetEnvironmentVariable("SALT")!);
        _encryptionKey = Environment.GetEnvironmentVariable("ENCRYPTION_KEY")!;
    }

    public static string Criptografar(string strTexto)
    {
        try
        {
            if (string.IsNullOrEmpty(strTexto))
            {
                return string.Empty;
            }

            using (var aesAlg = Aes.Create())
            {
                using (var key = new Rfc2898DeriveBytes(_encryptionKey, _salt, 10000, HashAlgorithmName.SHA256))
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
        catch (CryptographicException e)
        {
            // Tratar exceção criptográfica
            throw new ApplicationException("Erro na operação criptográfica.", e);
        }
        catch (Exception e)
        {
            // Tratar outras exceções
            throw new ApplicationException("Erro inesperado.", e);
        }
    }

    public static string Descriptografar(string strTextoCriptografado)
    {
        try
        {
            if (string.IsNullOrEmpty(strTextoCriptografado))
            {
                return string.Empty;
            }

            byte[] fullCipher = Convert.FromBase64String(strTextoCriptografado);

            using (var aesAlg = Aes.Create())
            {
                using (var key = new Rfc2898DeriveBytes(_encryptionKey, _salt, 10000, HashAlgorithmName.SHA256))
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
        catch (CryptographicException e)
        {
            // Tratar exceção criptográfica
            throw new ApplicationException("Erro na operação criptográfica.", e);
        }
        catch (Exception e)
        {
            // Tratar outras exceções
            throw new ApplicationException("Erro inesperado.", e);
        }
    }
}
