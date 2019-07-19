using System;
using System.Collections.Generic;
using Microsoft.Azure.Documents;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;

namespace DbUpdateFunctions
{
    public static class OnBillSaved
    {
        [FunctionName("OnBillSaved")]
        public static void Run([CosmosDBTrigger(
            databaseName: "Flatmatez",
            collectionName: "Bills",
            ConnectionStringSetting = "AccountEndpoint=https://flatmatez-db.documents.azure.com:443/;AccountKey=DuKRrW3NqwbAPkziVfAFSRoC1ChS4Tqv92VG4xTdfd0gy7eB2a6NO5uxLNsrYlBkLmbqIPnVxM4kIdLKHYYKwQ==;",
            LeaseCollectionName = "leases")]IReadOnlyList<Document> input, ILogger log)
        {
            if (input != null && input.Count > 0)
            {
                log.LogInformation("Documents modified " + input.Count);
                log.LogInformation("First document Id " + input[0].Id);
            }
        }
    }
}
