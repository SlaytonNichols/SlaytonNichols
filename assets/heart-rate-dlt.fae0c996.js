import{l as n,o as s,h as i,w as l,m as o,b as e}from"./app.265fcea8.js";const h=e("div",{class:"markdown-body"},[e("h2",null,"Heart Rate Delta Live Table Pipeline"),e("pre",null,[e("code",null,`-- Databricks notebook source
-- DBTITLE 1,Create Bronze Heart Rates Table
CREATE OR REFRESH STREAMING LIVE TABLE heartrates_raw
LOCATION "/mnt/bronze/heartrates_raw"
AS SELECT *
  FROM cloud_files(
    "/mnt/landing/ouraring/heartrates",
    "json",
    map("schema", "bpm INT, source STRING, timestamp STRING")
  );

-- COMMAND ----------

-- DBTITLE 1,Create Silver Heart Rates Table
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

-- COMMAND ----------

-- DBTITLE 1,Create Gold Heart Rates Table
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

-- COMMAND ----------


`)]),e("h3",null,"Bronze"),e("p",null,"The first table, heartrates_raw, is created using the CREATE OR REFRESH STREAMING LIVE TABLE statement. This table is a streaming table, which means that it is continuously updated with new data as it becomes available. The table is stored in the bronze layer of Databricks and is given the location \u201C/mnt/bronze/heartrates_raw\u201D. The data for the table is sourced from cloud_files, which is specified in the FROM clause. The data is in JSON format, and the schema for the table is specified as \u201Cbpm INT, source STRING, timestamp STRING\u201D."),e("h3",null,"Silver"),e("p",null,"The second table, heartrates_cleaned, is created using the CREATE OR REFRESH LIVE TABLE statement. The table is stored in the silver layer of Databricks and is given the location \u201C/mnt/silver/heartrates_cleaned\u201D. The data for the table is sourced from the heartrates_raw table, which is specified in the FROM clause. The data is transformed using the SELECT statement, which adds two new columns to the table: heartrateDate and heartrateTime. These columns are derived from the timestamp column in the heartrates_raw table using the to_date and date_format functions, respectively. The heartrates_cleaned table also includes data from two other tables: silver.date and bronze.time. These tables are joined to the heartrates_cleaned table using the JOIN clause and the to_date and date_format functions, respectively."),e("h3",null,"Gold"),e("p",null,"The third table, heartrates_curated, is created using the CREATE OR REFRESH LIVE TABLE statement, just like the heartrates_cleaned table. This table is stored in the gold layer of Databricks with the location \u201C/mnt/gold/heartrates_curated\u201D. The data for the table is sourced from the heartrates_cleaned table, which is specified in the FROM clause. The data is transformed using the SELECT statement, which adds two new columns to the table: date and AmPmString. These columns are derived from the heartrateDate and heartrateTime columns in the heartrates_cleaned table, respectively. The heartrates_curated table also includes data from the silver.date and bronze.time tables, which are joined to the heartrates_curated table using the JOIN clause.")],-1),b="Heart Rate Delta Live Tables Pipeline",E="A brief example of a multi-hop ETL architecture using Databricks SQL.",p="2022-10-20T00:00:00.000Z",_=["databricks","etl","sql","data-engineering"],R=!1,f=[{property:"og:title",content:"Heart Rate Delta Live Tables Pipeline"}],L={__name:"heart-rate-dlt",setup(d,{expose:a}){const t={title:"Heart Rate Delta Live Tables Pipeline",summary:"A brief example of a multi-hop ETL architecture using Databricks SQL.",date:"2022-10-20T00:00:00.000Z",tags:["databricks","etl","sql","data-engineering"],draft:!1,meta:[{property:"og:title",content:"Heart Rate Delta Live Tables Pipeline"}]};return a({frontmatter:t}),n({title:"Heart Rate Delta Live Tables Pipeline",meta:[{property:"og:title",content:"Heart Rate Delta Live Tables Pipeline"}]}),(m,T)=>{const r=o;return s(),i(r,{frontmatter:t},{default:l(()=>[h]),_:1})}}};export{p as date,L as default,R as draft,f as meta,E as summary,_ as tags,b as title};
