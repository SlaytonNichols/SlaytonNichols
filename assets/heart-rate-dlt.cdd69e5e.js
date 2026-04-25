import{l as s,o as l,h as i,w as o,m as d,b as e,d as t}from"./app.d23f7484.js";const h=e("div",{class:"markdown-body"},[e("h2",null,"Heart Rate Delta Live Tables Pipeline"),e("p",null,[t("This is a small pipeline I built to learn Delta Live Tables (DLT). It ingests heart rate data from an Oura Ring, lands it as raw JSON, and progressively refines it through three layers. The "),e("a",{href:"/posts/databricks-setup"},"Databricks workspace setup"),t(" covers how the underlying infrastructure was provisioned.")]),e("p",null,"The pattern is standard medallion architecture: raw ingestion \u2192 cleaned/typed \u2192 business-ready. DLT handles the orchestration, lineage tracking, and incremental processing \u2014 you just declare the tables and transformations."),e("hr"),e("h3",null,"Bronze \u2014 Raw Ingestion"),e("pre",null,[e("code",{class:"language-sql"},`CREATE OR REFRESH STREAMING LIVE TABLE heartrates_raw
LOCATION "/mnt/bronze/heartrates_raw"
AS SELECT *
  FROM cloud_files(
    "/mnt/landing/ouraring/heartrates",
    "json",
    map("schema", "bpm INT, source STRING, timestamp STRING")
  );
`)]),e("p",null,[t("Auto Loader ("),e("code",null,"cloud_files"),t(") picks up new JSON files as they land in the mount. The streaming table processes incrementally \u2014 only new files on each run. No parsing, no transforms, just land the data.")]),e("hr"),e("h3",null,"Silver \u2014 Cleaned & Typed"),e("pre",null,[e("code",{class:"language-sql"},`CREATE OR REFRESH LIVE TABLE heartrates_cleaned
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
`)]),e("p",null,"Split the raw timestamp into date and time components, and join against shared dimension tables. This is where malformed or orphan records get filtered out \u2014 anything that doesn\u2019t join drops silently."),e("hr"),e("h3",null,"Gold \u2014 Business-Ready"),e("pre",null,[e("code",{class:"language-sql"},`CREATE OR REFRESH LIVE TABLE heartrates_curated
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
`)]),e("p",null,[t("The curated table enriches with business-friendly fields ("),e("code",null,"isWeekDay"),t(", "),e("code",null,"AmPmString"),t(") and drops everything a downstream consumer doesn\u2019t need. This is the table you\u2019d point a dashboard or notebook at.")]),e("hr"),e("h3",null,"What I Took Away"),e("p",null,[t("DLT is convenient for small pipelines like this \u2014 you get dependency resolution, incremental processing, and lineage for free. The tradeoff is less control over execution details compared to writing Spark jobs directly. For anything more complex (like the "),e("a",{href:"/posts/trading-bot-journey"},"trading system pipeline"),t("), I ended up needing that control.")])],-1),T="Heart Rate Delta Live Tables Pipeline",b="A multi-hop ETL pipeline built with Databricks Delta Live Tables \u2014 bronze/silver/gold layers for Oura Ring heart rate data.",f="2022-10-20T00:00:00.000Z",y=["databricks","etl","sql","data-engineering"],_=!1,L=[{property:"og:title",content:"Heart Rate Delta Live Tables Pipeline"}],R={__name:"heart-rate-dlt",setup(c,{expose:n}){const a={title:"Heart Rate Delta Live Tables Pipeline",summary:"A multi-hop ETL pipeline built with Databricks Delta Live Tables \u2014 bronze/silver/gold layers for Oura Ring heart rate data.",date:"2022-10-20T00:00:00.000Z",tags:["databricks","etl","sql","data-engineering"],draft:!1,meta:[{property:"og:title",content:"Heart Rate Delta Live Tables Pipeline"}]};return n({frontmatter:a}),s({title:"Heart Rate Delta Live Tables Pipeline",meta:[{property:"og:title",content:"Heart Rate Delta Live Tables Pipeline"}]}),(m,u)=>{const r=d;return l(),i(r,{frontmatter:a},{default:o(()=>[h]),_:1})}}};export{f as date,R as default,_ as draft,L as meta,b as summary,y as tags,T as title};
