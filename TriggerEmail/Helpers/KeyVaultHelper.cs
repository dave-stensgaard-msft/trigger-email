using Onyx.KeyVault;
using System;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using TriggerEmail.Entities;

namespace TriggerEmail.Helpers
{
    public class KeyVaultHelper
    {
        public const string ClientIdEnvironmentVariable = "KeyholdR_ClientId";
        public const string SecretEnvironmentVariable = "KeyholdR_Secret";
        // Azure Powershell client ID for interactive authentication
        private const string InteractiveAuthClientId = "1950a258-227b-4e31-a9cf-717495945fc2";

        private readonly Regex configRegex = new Regex(@"(?<keyvault>(?<=\#{keyvault:)[^}]*(?=\}))", RegexOptions.IgnoreCase | RegexOptions.Singleline);

        private readonly string clientId;
        private readonly string secret;
        private readonly string certificateThumbprint;
        private readonly string directoryId;

        private KeyvaultOperations keyVaultOperations;

        public KeyVaultHelper(Configuration configuration)
        {
            this.clientId = configuration.KeyVaultClientId;
            this.secret = configuration.KeyVaultSecret;
            this.certificateThumbprint = configuration.KeyVaultCertificateThumbprint;
            this.directoryId = configuration.KeyVaultDirectoryId;
        }

        public static string GetEnvironmentVariableOrSettings(string environmentVariable, string settingsValue)
        {
            return Environment.GetEnvironmentVariable(environmentVariable) ?? settingsValue;
        }

        public KeyvaultOperations GetKeyVaultOperations()
        {
            if (this.keyVaultOperations == null)
            {
                this.keyVaultOperations = this.CreateKeyVaultOperations(
                GetEnvironmentVariableOrSettings(ClientIdEnvironmentVariable, this.clientId),
                GetEnvironmentVariableOrSettings(SecretEnvironmentVariable, this.secret),
                this.certificateThumbprint,
                this.directoryId);
            }

            return this.keyVaultOperations;
        }

        public string ResolveKeyVaultPlaceholder(string placeholder)
        {
            var value = placeholder;

            var regexMatch = this.configRegex.Match(placeholder);
            if (regexMatch.Success)
            {
                var keyMatch = regexMatch.Groups["keyvault"].ToString().Split(':');

                if (keyMatch.Length > 2)
                {
                    throw new Exception($"The {nameof(placeholder)}: {placeholder} is not in correct format.");
                }

                try
                {
                    // Get value from keyvault
                    value = Task.Run(() => this.GetKeyVaultOperations().GetSecretAsync(keyMatch[0], keyMatch[1])).Result;
                    Console.WriteLine($"Secret '{keyMatch[1]}' retrieved from vault '{keyMatch[0]}'");
                }
                catch (AggregateException ex)
                {
                    var errorMessages = string.Empty;
                    foreach (var innerException in ex.Flatten().InnerExceptions)
                    {
                        errorMessages += innerException.Message + Environment.NewLine;
                    }

                    throw new Exception($"An error ocurred while retrieving '{keyMatch[1]}' from vault '{keyMatch[0]}': {errorMessages}");
                }
            }

            return value;
        }

        private KeyvaultOperations CreateKeyVaultOperations(string clientId, string secret, string thumbprint, string directoryId)
        {
            var authMethod =
                (string.IsNullOrWhiteSpace(thumbprint) && string.IsNullOrWhiteSpace(secret)) ? AuthMethod.Interactive
                : string.IsNullOrWhiteSpace(thumbprint) ? AuthMethod.ServicePrincipal : AuthMethod.Certificate;

            return new KeyvaultOperations(
                authMethod,
                authMethod == AuthMethod.Interactive ? InteractiveAuthClientId : clientId,
                certThumbprintOrSecret: !string.IsNullOrWhiteSpace(thumbprint) ? thumbprint : secret,
                directoryId: string.IsNullOrWhiteSpace(directoryId) ? null : directoryId);
        }
    }
}
