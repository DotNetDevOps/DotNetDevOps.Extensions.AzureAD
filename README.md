
# DotNetDevOps.Extensions.AzureAD


# DotNetDevOps.Extensions.AzureAD.Publisher

A Azure function (DotNetCore 3.1) that every 5min scans AzureAD users for changes and publishes on eventgrid domain or topic.

When using a Event Grid Domain, provide the resource id for the domain, and it will auto create topic if it do not exist.

local.settings.
```
{
    "IsEncrypted": false,
    "Values": {
        "AzureWebJobsStorage": "UseDevelopmentStorage=true",
        "FUNCTIONS_WORKER_RUNTIME": "dotnet",
        "EventGrid__Key": "=",
        "EventGrid__Endpoint": "",
        "EventGrid__ResourceId": "",
        "EventGrid__TopicName":""
    }
}
```