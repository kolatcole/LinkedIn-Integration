The following are the required json format for running the application



Post Text Only
{

"author": "urn:li:organization:80409542", // the organization urn that you represent
  "commentary": "Sample text Post",
  "visibility": "PUBLIC",
  "distribution": {
    "feedDistribution": "MAIN_FEED"
  },
  "lifecycleState": "PUBLISHED",
  "isReshareDisabledByAuthor": false
}

Post with one image or video
{
  "author": "urn:li:organization:80409542",
  "commentary": "Sample text Post",
  "visibility": "PUBLIC",
  "distribution": {
    "feedDistribution": "MAIN_FEED",
    "targetEntities": [],
    "thirdPartyDistributionChannels": []
  },
  "lifecycleState": "PUBLISHED",
  "isReshareDisabledByAuthor": false,
  "content": {
    "media": {
      "title": "https://cdn.pixabay.com/photo/2016/10/26/19/00/domain-names-1772240_1280.png", // this should have the video or image url
      "id": "string"
    }
  }
}

Post with multiple images
{
  "author": "urn:li:organization:80409542",
  "commentary": "Sample text Post",
  "visibility": "PUBLIC",
  "distribution": {
    "feedDistribution": "MAIN_FEED",
    "targetEntities": [],
    "thirdPartyDistributionChannels": []
  },
  "lifecycleState": "PUBLISHED",
  "isReshareDisabledByAuthor": false,
  "content": {
    "media": {
      "title": "https://cdn.pixabay.com/photo/2016/10/26/19/00/domain-names-1772240_1280.png,https://cdn.pixabay.com/photo/2016/10/26/19/00/domain-names-1772240_1280.png", // this should have the  image urls separated by comma
      "id": "string"
    }
  }
}

// Repost
{
  "author": "urn:li:organization:80409542", // the organization urn that you represent
  "commentary": "Sample text Post",
  "visibility": "PUBLIC",
  "distribution": {
    "feedDistribution": "MAIN_FEED"
  },
  "lifecycleState": "PUBLISHED",
  "isReshareDisabledByAuthor": false,
  "reshareContext":  {
    "parent": "urn:li:share:7160215790661611522"  // the post urn that you intend to repost
  }

// Update post
{
  "patch": {
        "$set": {
            "commentary": "Update to the post",
            "contentCallToActionLabel": "LEARN_MORE"
        }
    }
}

  //create comment
{
   "actor":"urn:li:organization:80409542",
   "message":{
      "text":"commentV4 with image entity"
   }
}
}
