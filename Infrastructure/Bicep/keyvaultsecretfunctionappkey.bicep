// --------------------------------------------------------------------------------
// This BICEP file will create KeyVault secret for a function app key
// --------------------------------------------------------------------------------
param keyVaultName string = ''
param keyName string = ''
param functionAppName string = ''

// --------------------------------------------------------------------------------
resource functionAppResource 'Microsoft.Web/sites@2021-03-01' existing = { name: functionAppName }
var defaultHostKey = listkeys('${functionAppResource.id}/host/default', '2016-08-01').functionKeys.default

// --------------------------------------------------------------------------------
resource keyvaultResource 'Microsoft.KeyVault/vaults@2021-11-01-preview' existing = { 
  name: keyVaultName
  resource storageSecret 'secrets' = {
    name: keyName
    properties: {
      value: defaultHostKey
    }
  }
}
