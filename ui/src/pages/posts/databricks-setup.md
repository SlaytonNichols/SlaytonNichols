---
title: "Databricks Workspace Setup"
summary: "Standing up a Databricks workspace on Azure with ADLS Gen2, service principal auth, and Unity Catalog — from scratch."
date: 2022-10-19
tags:
  - databricks
  - cloud
  - data-engineering
draft: false
---
## Databricks Workspace Setup

Notes from standing up a Databricks workspace on Azure, wired to ADLS Gen2 storage with service principal authentication. This was the foundation for everything that came after — the [Heart Rate DLT pipeline](/posts/heart-rate-dlt) and later the orderbook data platform for the [trading project](/posts/trading-bot-journey).

---

### 1. Provision the Azure Resources

**Resource Group** — Create a resource group to keep everything together. Region matters for latency and compliance; pick the one closest to your data sources.

**ADLS Gen2 Storage Account** — This is where your raw data lands and where Databricks writes managed tables.
- Create a Storage Account with **Hierarchical Namespace enabled** (this is what makes it Gen2 vs. regular blob storage)
- Create containers for your data layers. I used `landing`, `bronze`, `silver`, and `gold` — the standard medallion architecture
- Enable soft delete for blobs if you want a safety net during development

**Databricks Workspace** — Deploy an Azure Databricks workspace from the marketplace. The Premium tier is required if you want Unity Catalog, which you probably do.

---

### 2. Create an Azure AD App Registration & Service Principal

Databricks needs a way to authenticate to your storage account. A service principal is cleaner than passing around personal credentials or storage keys.

1. Go to **Azure Active Directory → App registrations → New registration**
2. Name it something obvious (e.g., `databricks-adls-sp`)
3. After creation, note the **Application (client) ID** and **Directory (tenant) ID**
4. Under **Certificates & secrets**, create a new client secret. Copy the value immediately — you won't see it again

Reference: [Creating AD App and Service Principal](https://learn.microsoft.com/en-us/azure/active-directory/develop/howto-create-service-principal-portal)

---

### 3. Assign Storage Permissions

The service principal needs an IAM role on the storage account so it can read and write data.

1. Navigate to your ADLS Gen2 Storage Account → **Access Control (IAM)**
2. Add a role assignment: **Storage Blob Data Contributor**
3. Assign it to the service principal you just created

`Storage Blob Data Contributor` gives read, write, and delete on blob data. If you only need read access for a particular use case, use `Storage Blob Data Reader` instead.

Reference: [Assign an Azure role for access to blob data](https://learn.microsoft.com/en-us/azure/storage/blobs/assign-azure-role-data-access?tabs=portal)

---

### 4. Configure Databricks to Access Storage

Store your service principal credentials in Databricks so notebooks and jobs can mount or directly access ADLS.

**Option A: Spark config (direct access, simpler)**

Set these in your cluster's Spark configuration or at the notebook level:

```
spark.conf.set("fs.azure.account.auth.type.<storage-account>.dfs.core.windows.net", "OAuth")
spark.conf.set("fs.azure.account.oauth.provider.type.<storage-account>.dfs.core.windows.net",
  "org.apache.hadoop.fs.azurebfs.oauth2.ClientCredsTokenProvider")
spark.conf.set("fs.azure.account.oauth2.client.id.<storage-account>.dfs.core.windows.net", "<client-id>")
spark.conf.set("fs.azure.account.oauth2.client.secret.<storage-account>.dfs.core.windows.net", "<client-secret>")
spark.conf.set("fs.azure.account.oauth2.client.endpoint.<storage-account>.dfs.core.windows.net",
  "https://login.microsoftonline.com/<tenant-id>/oauth2/token")
```

Then reference paths directly: `abfss://bronze@<storage-account>.dfs.core.windows.net/heartrates_raw`

**Option B: DBFS mount (legacy, but still common)**

```python
configs = {
  "fs.azure.account.auth.type": "OAuth",
  "fs.azure.account.oauth.provider.type":
    "org.apache.hadoop.fs.azurebfs.oauth2.ClientCredsTokenProvider",
  "fs.azure.account.oauth2.client.id": "<client-id>",
  "fs.azure.account.oauth2.client.secret": "<client-secret>",
  "fs.azure.account.oauth2.client.endpoint":
    "https://login.microsoftonline.com/<tenant-id>/oauth2/token"
}

dbutils.fs.mount(
  source="abfss://bronze@<storage-account>.dfs.core.windows.net/",
  mount_point="/mnt/bronze",
  extra_configs=configs
)
```

Repeat for each container (`/mnt/silver`, `/mnt/gold`, etc.). Mounts persist across cluster restarts.

> **Note:** Databricks recommends direct access with Unity Catalog external locations over mounts for new workspaces. Mounts work fine, but Unity Catalog gives you centralized governance and audit logging.

---

### 5. Store Secrets Properly

Don't hardcode the client secret in notebooks. Use **Databricks Secret Scopes** backed by Azure Key Vault:

1. Create an Azure Key Vault and add the client secret as a secret
2. In Databricks, create a secret scope backed by that Key Vault
3. Reference it in code: `dbutils.secrets.get(scope="my-scope", key="sp-client-secret")`

This keeps credentials out of version control and notebook history.

---

### 6. Validate the Setup

Quick smoke test once everything is wired up:

```python
# List files in the landing container
dbutils.fs.ls("/mnt/landing")

# Read a sample file
df = spark.read.json("/mnt/landing/ouraring/heartrates")
df.show(5)
```

If you get an authentication error, check:
- The service principal has the correct role on the **storage account** (not just the resource group)
- The client secret hasn't expired
- The tenant ID and client ID are correct

---

### What Came Next

With storage accessible from Databricks, the next step was building pipelines on top of it. The [Heart Rate DLT pipeline](/posts/heart-rate-dlt) was the first — a simple bronze/silver/gold flow using Delta Live Tables to process Oura Ring heart rate data through the medallion architecture.
