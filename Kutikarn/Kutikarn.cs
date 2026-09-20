using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace Kutikarn
{
    internal class Kutikarn
    {
        private bool _d;

        public Kutikarn(bool jhanda)
        {
            _d = jhanda;   
        }
        
        internal string Kuti(string naam, string kunji)
        {
            if (File.Exists(naam))
            {
                KutiFile(naam, kunji);
            }
            else
            {
                string[] files = Directory.GetFiles(naam);
                foreach (string file in files)
                    {
                    KutiFile(file, kunji);
                    }
                }
            return String.Empty;
        }

        private void KutiFile(string file, string kunji)
        {
            // Create a string to encrypt.
            string bahar = Path.Combine(Path.GetFullPath(file).Replace(Path.GetFileName(file), ""), Path.GetFileName(file) + ".kkf");
            byte[] Key = KeyBytes(kunji);
            // Encrypt text to a file using the file name, key, and IV.
            EncryptBinaryToFile(file, bahar, Key);
            if (_d == true)
            {
                File.Delete(file);
            }
        }

        internal string Duti(string naam, string kunji)
        {
            if (File.Exists(naam))
            {
                DutiFile(naam, kunji);
            }
            else
            {
                string[] files = Directory.GetFiles(naam);
                foreach (string file in files)
                    {
                    DutiFile(file, kunji);
                    }
                }
            return String.Empty;
        }

        private void DutiFile(string file, string kunji)
        {
            string bahar = Path.Combine(Path.GetFullPath(file).Replace(Path.GetFileName(file), ""), Path.GetFileName(file).Replace(".kkf",""));
            byte[] Key = KeyBytes(kunji);
            // Decrypt the text from a file using the file name, key, and IV.
            DecryptBinaryFromFile(file, bahar, Key);
            if (_d == true)
            {
                File.Delete(file);
            }
        }

        private void EncryptBinaryToFile(string inputFile, string outputFile, byte[] key)
        {
            try
            {
                using (var tdes = TripleDES.Create())
                {
                    tdes.Key = key;
                    tdes.IV = IV(); // 8 bytes — correct for TripleDES

                    using (FileStream fsOut = File.Create(outputFile))
                    using (CryptoStream cs = new CryptoStream(fsOut, tdes.CreateEncryptor(), CryptoStreamMode.Write))
                    using (FileStream fsIn = File.OpenRead(inputFile))
                    {
                        fsIn.CopyTo(cs);
                    }
                }
            }
            catch (CryptographicException e)
            {
                Console.WriteLine($"A Cryptographic error occurred: {e.Message}");
            }
            catch (UnauthorizedAccessException e)
            {
                Console.WriteLine($"A file access error occurred: {e.Message}");
            }
        }


        private void DecryptBinaryFromFile(string inputFile, string outputFile, byte[] key)
        {
            try
            {
                using (var tdes = TripleDES.Create())
                {
                    tdes.Key = key;
                    tdes.IV = IV(); // fixed 8-byte IV

                    using (FileStream fsIn = File.OpenRead(inputFile))
                    using (CryptoStream cs = new CryptoStream(fsIn, tdes.CreateDecryptor(), CryptoStreamMode.Read))
                    using (FileStream fsOut = File.Create(outputFile))
                    {
                        cs.CopyTo(fsOut);
                    }
                }
            }
            catch (CryptographicException e)
            {
                Console.WriteLine($"A Cryptographic error occurred: {e.Message}");
            }
            catch (UnauthorizedAccessException e)
            {
                Console.WriteLine($"A file access error occurred: {e.Message}");
            }
        }


        [System.Reflection.Obfuscation(Feature = "virtualization", Exclude = false)]
        private byte[] KeyBytes(string key)
        {
            // TripleDES supports 128-bit (16 bytes) or 192-bit (24 bytes) keys.
            const int Key16 = 16;
            const int Key24 = 24;

            var padChars = ChArr();

            if (key.Length < Key16)
            {
                // Pad up to 16 chars
                key = key.PadRight(Key16, padChars[0]);
            }
            else if (key.Length > Key16 && key.Length < Key24)
            {
                // Pad up to 24 chars
                key = key.PadRight(Key24, padChars[0]);
            }
            else if (key.Length > Key24)
            {
                // Truncate to 24 chars
                key = key.Substring(0, Key24);
            }

            return Encoding.UTF8.GetBytes(key);
        }

        [System.Reflection.Obfuscation(Feature = "virtualization", Exclude = false)]
        private char[] ChArr()
        {
            return "Bg82an&%2hvbOA*d".ToCharArray();
        }

        [System.Reflection.Obfuscation(Feature = "virtualization", Exclude = false)]
        private byte[] IV()
        {
            // TripleDES requires an 8-byte IV (64-bit block size)
            return new byte[] { 0xB1, 0xC5, 0x60, 0x05, 0x59, 0x13, 0x4E, 0x08 };
        }

    }
}
