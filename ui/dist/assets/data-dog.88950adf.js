import{l as s,o as n,h as r,w as c,m as i,b as e}from"./app.333304cd.js";const g=e("div",{class:"markdown-body"},[e("h2",null,"Data Dog Logging Setup"),e("pre",null,[e("code",{class:"language-yml"},`version: "3.9"
services:
  service-1:
    labels:
      com.datadoghq.ad.logs: '[{"source": "docker", "service": "service-1"}]'
  service-2:
    labels:
      com.datadoghq.ad.logs: '[{"source": "docker", "service": "service-2"}]'
  datadog:
    image: gcr.io/datadoghq/agent:7
    pid: host
    environment:
      - DD_API_KEY=\${DD_API_KEY}
      - DD_SITE=\${DD_SITE}
      - DD_LOGS_ENABLED=true
    volumes:
      - /var/run/docker.sock:/var/run/docker.sock
      - /proc/:/host/proc:ro
      - /sys/fs/cgroup/:/host/sys/fs/cgroup:ro
`)]),e("p",null,"This is an example docker-compose file that defines one service that runs the datadog agent alongside two other services. The datadog container is based on the image gcr.io/datadoghq/agent:7, which is the official datadog agent image. The service-1 and service-2 services are both labeled with the com.datadoghq.ad.logs label. This label is used to specify the logs that the Datadog agent should collect. In this case, the com.datadoghq.ad.logs label is set to \u2018[{\u201Csource\u201D: \u201Cdocker\u201D, \u201Cservice\u201D: \u201Cservice-1\u201D}]\u2019 for the service-1 service, and \u2018[{\u201Csource\u201D: \u201Cdocker\u201D, \u201Cservice\u201D: \u201Cservice-2\u201D}]\u2019 for the service-2 service. This tells the Datadog agent to collect logs from the service-1 and service-2 services, respectively."),e("p",null,"The pid property is set to host, which means that the container will share the host\u2019s PID namespace. This is useful for monitoring processes running on the host, as it allows the Datadog agent to see the host\u2019s processes as if they were running inside the container."),e("p",null,"The environment property is used to set environment variables for the container. In this case, three environment variables are set: DD_API_KEY, DD_SITE, and DD_LOGS_ENABLED. The DD_API_KEY and DD_SITE variables are used to authenticate the Datadog agent with your Datadog account. The DD_LOGS_ENABLED variable is used to enable log collection in the Datadog agent."),e("p",null,"The volumes property is used to mount host directories or files as volumes in the container. In this case, three volumes are mounted: /var/run/docker.sock, /proc/, and /sys/fs/cgroup/. The /var/run/docker.sock volume is used to allow the Datadog agent to collect logs from other containers running on the host. The /proc/ and /sys/fs/cgroup/ volumes are used to allow the Datadog agent to collect metrics from the host\u2019s processes and system.")],-1),D="Data Dog Logging Setup",v="Data Dog Logging Setup",m="2022-10-19T00:00:00.000Z",_=["devops","observability","docker","datadog"],f=!0,y=[{property:"og:title",content:"Data Dog Logging Setup"}],b={__name:"data-dog",setup(d,{expose:o}){const t={title:"Data Dog Logging Setup",summary:"Data Dog Logging Setup",date:"2022-10-19T00:00:00.000Z",tags:["devops","observability","docker","datadog"],draft:!0,meta:[{property:"og:title",content:"Data Dog Logging Setup"}]};return o({frontmatter:t}),s({title:"Data Dog Logging Setup",meta:[{property:"og:title",content:"Data Dog Logging Setup"}]}),(h,u)=>{const a=i;return n(),r(a,{frontmatter:t},{default:c(()=>[g]),_:1})}}};export{m as date,b as default,f as draft,y as meta,v as summary,_ as tags,D as title};
