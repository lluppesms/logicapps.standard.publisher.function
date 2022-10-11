// --------------------------------------------------------------------------------
// Main file that deploys all Azure Resources for one environment
// --------------------------------------------------------------------------------
// NOTE: To make this pipeline work, your service principal may need to be in the
//   "acr pull" role for the container registry.
// --------------------------------------------------------------------------------
// To deploy this Bicep manually:
// 	 az login
//   az account set --subscription <subscriptionId>
//   az deployment group create -n main-deploy-20221011T150000Z --resource-group rg_las_publish_demo --template-file 'main.bicep' --parameters orgPrefix=lll appPrefix=laspub environmentCode=demo keyVaultOwnerUserId1=xxxxxxxx-xxxx-xxxx
// --------------------------------------------------------------------------------
param environmentCode string = 'demo'
param location string = resourceGroup().location
param orgPrefix string = 'org'
param appPrefix string = 'app'
param appSuffix string = '' // '-1' 
param storageSku string = 'Standard_LRS'
param functionName string = 'func'
param functionAppSku string = 'Y1'
param functionAppSkuFamily string = 'Y'
param functionAppSkuTier string = 'Dynamic'
param keyVaultOwnerUserId1 string = ''
param azDoOrganization string = ''
param azDoPatToken string = ''
param azDoProject string = ''
param refreshPipelineName string = ''
param runDateTime string = utcNow()

// --------------------------------------------------------------------------------
var deploymentSuffix = '-${runDateTime}'

module storageModule 'storageaccount.bicep' = {
  name: 'storage${deploymentSuffix}'
  params: {
    storageSku: storageSku

    templateFileName: 'storageaccount.bicep'
    orgPrefix: orgPrefix
    appPrefix: appPrefix
    environmentCode: environmentCode
    appSuffix: appSuffix
    location: location
    runDateTime: runDateTime
  }
}

module functionModule 'functionapp.bicep' = {
  name: 'function${deploymentSuffix}'
  dependsOn: [ storageModule ]
  params: {
    functionName: functionName
    functionKind: 'functionapp,linux'
    functionAppSku: functionAppSku
    functionAppSkuFamily: functionAppSkuFamily
    functionAppSkuTier: functionAppSkuTier
    functionStorageAccountName: storageModule.outputs.storageAccountName
    appInsightsLocation: location

    templateFileName: 'functionapp.bicep'
    orgPrefix: orgPrefix
    appPrefix: appPrefix
    environmentCode: environmentCode
    appSuffix: appSuffix
    location: location
    runDateTime: runDateTime
  }
}
module keyVaultModule 'keyvault.bicep' = {
  name: 'keyvault${deploymentSuffix}'
  dependsOn: [ functionModule ]
  params: {
    adminUserObjectIds: [ keyVaultOwnerUserId1 ]
    applicationUserObjectIds: [ functionModule.outputs.functionAppPrincipalId ]

    templateFileName: 'keyvault.bicep'
    orgPrefix: orgPrefix
    appPrefix: appPrefix
    environmentCode: environmentCode
    appSuffix: appSuffix
    location: location
    runDateTime: runDateTime
  }
}
module keyVaultSecret1 'keyvaultsecret.bicep' = {
  name: 'keyVaultSecret1${deploymentSuffix}'
  dependsOn: [ keyVaultModule, functionModule ]
  params: {
    keyVaultName: keyVaultModule.outputs.keyVaultName
    secretName: 'functionAppInsightsKey'
    secretValue: functionModule.outputs.functionInsightsKey
  }
}

module keyVaultSecret2 'keyvaultsecretstorageconnection.bicep' = {
  name: 'keyVaultSecret2${deploymentSuffix}'
  dependsOn: [ keyVaultModule, storageModule ]
  params: {
    keyVaultName: keyVaultModule.outputs.keyVaultName
    keyName: 'functionStorageAccountConnectionString'
    storageAccountName: storageModule.outputs.storageAccountName
  }
}
module keyVaultSecret3 'keyvaultsecret.bicep' = {
  name: 'keyVaultSecret3${deploymentSuffix}'
  dependsOn: [ keyVaultModule, functionModule ]
  params: {
    keyVaultName: keyVaultModule.outputs.keyVaultName
    secretName: 'AzDoOrganization'
    secretValue: azDoOrganization
  }
}
module keyVaultSecret4 'keyvaultsecret.bicep' = {
  name: 'keyVaultSecret4${deploymentSuffix}'
  dependsOn: [ keyVaultModule, functionModule ]
  params: {
    keyVaultName: keyVaultModule.outputs.keyVaultName
    secretName: 'AzDoPatToken'
    secretValue: azDoPatToken
  }
}
module keyVaultSecret5 'keyvaultsecret.bicep' = {
  name: 'keyVaultSecret5${deploymentSuffix}'
  dependsOn: [ keyVaultModule, functionModule ]
  params: {
    keyVaultName: keyVaultModule.outputs.keyVaultName
    secretName: 'AzDoProject'
    secretValue: azDoProject
  }
}
module keyVaultSecret6 'keyvaultsecret.bicep' = {
  name: 'keyVaultSecret6${deploymentSuffix}'
  dependsOn: [ keyVaultModule, functionModule ]
  params: {
    keyVaultName: keyVaultModule.outputs.keyVaultName
    secretName: 'RefreshPipelineName'
    secretValue: refreshPipelineName
  }
}


module functionAppSettingsModule 'functionappsettings.bicep' = {
  name: 'functionAppSettings${deploymentSuffix}'
  dependsOn: [ keyVaultSecret1, keyVaultSecret2, keyVaultSecret3, keyVaultSecret4, keyVaultSecret5, keyVaultSecret6, functionModule ]
  params: {
    functionAppName: functionModule.outputs.functionAppName
    functionStorageAccountName: functionModule.outputs.functionStorageAccountName
    functionInsightsKey: functionModule.outputs.functionInsightsKey
    customAppSettings: {
      AzDoOrganizationReference: '@Microsoft.KeyVault(VaultName=${keyVaultModule.outputs.keyVaultName};SecretName=AzDoOrganization)'
      AzDoPatTokenReference: '@Microsoft.KeyVault(VaultName=${keyVaultModule.outputs.keyVaultName};SecretName=AzDoPatToken)'
      AzDoProjectReference: '@Microsoft.KeyVault(VaultName=${keyVaultModule.outputs.keyVaultName};SecretName=AzDoProject)'
      RefreshPipelineNameReference: '@Microsoft.KeyVault(VaultName=${keyVaultModule.outputs.keyVaultName};SecretName=RefreshPipelineName)'
    }
  }
}
