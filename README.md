# VideoIndexerExample

This project is an example of difference ways i've tried to use the v2 Video Indexer API so i can get some assistance from the Video Indexer development team

### Prerequisites

You will need a Video Indexer account with videos already uploaded to it.

## Getting Started

To use, go to Controllers/HomeController.cs, line 13, and input your APIKey, Location, and AccountId.

```
internal VIServiceWrapper vi = new VIServiceWrapper("apiKey", "location", "accountId", "https://api.videoindexer.ai/");
```
