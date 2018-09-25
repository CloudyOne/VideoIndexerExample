# VideoIndexerExample

This project is an example of how to use the v2 Video Indexer API for doing player and insight embeds

### Prerequisites

You will need a [Video Indexer Account](https://api-portal.videoindexer.ai/) with videos already uploaded to it (which requires you to have a Azure Media Service resource attached to your account)

## Getting Started

To use, go to Controllers/HomeController.cs, line 13, and input your AccountId, APIKey, Location, and Location Subdomain.

```
internal VIServiceWrapper vi = new VIServiceWrapper("accountID Guid","apiKey", "location, ie. westus2","location subdomain, ie. wus2", "https://api.videoindexer.ai/");
```
* The *AccountId* is found from your [Video Indexer Account Settings](https://www.videoindexer.ai/settings/account)
* The *ApiKey* is found from your [Developer Account Profile](https://api-portal.videoindexer.ai/developer)
* The *Location* is the location of your Azure Media Service resource on your Azure Account, lowercase, without spaces. ie. "West US 2" becomes "westus2"
* The *Location Subdomain* can be found from your Video Indexer account ![Location Subdomain](https://i.imgur.com/UznDZdT.png) 
