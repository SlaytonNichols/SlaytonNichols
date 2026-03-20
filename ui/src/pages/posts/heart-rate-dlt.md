---
title: "Heart Rate Delta Live Tables Pipeline"
summary: "A multi-hop ETL pipeline built with Databricks Delta Live Tables — bronze/silver/gold layers for Oura Ring heart rate data."
date: 2022-10-20
tags:
  - databricks
  - etl
  - sql
  - data-engineering
draft: false
---
## Heart Rate Delta Live Tables Pipeline

This is a small pipeline I built to learn Delta Live Tables (DLT). It ingests heart rate data from an Oura Ring, lands it as raw JSON, and progressively refines it through three layers. The [Databricks workspace setup](/posts/databricks-setup) covers how the underlying infrastructure was provisioned.

The pattern is standard medallion architecture: raw ingestion → cleaned/typed → business-ready. DLT handles the orchestration, lineage tracking, and incremental processing — you just declare the tables and transformations.

---

### Bronze — Raw Ingestion

```sql
CREATE OR REFRESH STREAMING LIVE TABLE heartrates_raw
LOCATION "/mnt/bronze/heartrates_raw"
AS SELECT *
  FROM cloud_files(
    "/mnt/landing/ouraring/heartrates",
    "json",
    map("schema", "bpm INT, source STRING, timestamp STRING")
  );
```

Auto Loader (`cloud_files`) picks up new JSON files as they land in the mount. The streaming table processes incrementally — only new files on each run. No parsing, no transforms, just land the data.

---

### Silver — Cleaned & Typed

```sql
CREATE OR REFRESH LIVE TABLE heartrates_cleaned
LOCATION "/mnt/silver/heartrates_cleaned"
AS
SELECT 
  h.bpm,
  h.source,  
  to_date(h.timestamp) as heartrateDate,
  date_format(h.timestamp, "HH:mm:ss") as heartrateTime
FROM LIVE.heartrates_raw h
JOIN silver.date d on to_date(h.timestamp) = d.date
JOIN bronze.time t on date_format(h.timestamp, "HH:mm:ss") = t.FullTime
```

Split the raw timestamp into date and time components, and join against shared dimension tables. This is where malformed or orphan records get filtered out — anything that doesn't join drops silently.

---

### Gold — Business-Ready

```sql
CREATE OR REFRESH LIVE TABLE heartrates_curated
LOCATION "/mnt/gold/heartrates_curated"
AS
SELECT 
  h.bpm,
  d.date, 
  d.isWeekDay,
  t.AmPmString
FROM LIVE.heartrates_cleaned h
JOIN silver.date d on h.heartrateDate = d.date
JOIN bronze.time t on h.heartrateTime = t.FullTime
```

The curated table enriches with business-friendly fields (`isWeekDay`, `AmPmString`) and drops everything a downstream consumer doesn't need. This is the table you'd point a dashboard or notebook at.

---

### What I Took Away

DLT is convenient for small pipelines like this — you get dependency resolution, incremental processing, and lineage for free. The tradeoff is less control over execution details compared to writing Spark jobs directly. For anything more complex (like the [trading system pipeline](/posts/trading-bot-journey)), I ended up needing that control.
