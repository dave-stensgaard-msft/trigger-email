using System;
using System.Configuration;

namespace TriggerEmail.Entities
{
    public class Configuration
    {
        public static Configuration ReadConfiguration()
        {
            try
            {
                return new Configuration
                {
                    AppId = ConfigurationManager.AppSettings["TriggerEmail.Appid"],
                    Secret = ConfigurationManager.AppSettings["TriggerEMail.Secret"],
                    KeyVaultClientId = ConfigurationManager.AppSettings["KeyVault.ClientId"],
                    KeyVaultSecret = ConfigurationManager.AppSettings["KeyVault.Secret"],
                    KeyVaultCertificateThumbprint = ConfigurationManager.AppSettings["KeyVault.CertificateThumbprint"],
                    KeyVaultDirectoryId = ConfigurationManager.AppSettings["KeyVault.DirectoryId"],
                    SeqServer = ConfigurationManager.AppSettings["SeqServer"],
                    SeqServerApiKey = ConfigurationManager.AppSettings["SeqServerApiKey"]
                };
            }
            catch (Exception ex)
            {
                throw new Exception($"An error occurred while reading the configuration. Details: {ex.Message}", ex);
            }
        }

        public string AppId { get; set; }

        public string Secret { get; set; }

        public string KeyVaultClientId { get; set; }

        public string KeyVaultSecret { get; set; }

        public string KeyVaultCertificateThumbprint { get; set; }

        public string KeyVaultDirectoryId { get; set; }

        public string SeqServer { get; set; }

        public string SeqServerApiKey { get; set; }
    }
}
