---
title: "Data Dog Logging Setup"
summary: "Data Dog Logging Setup"
date: 2022-10-19
tags:
  - devops
  - observability
  - docker
  - datadog
draft: true
---
## Data Dog Logging Setup

```yml
version: "3.9"
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
      - DD_API_KEY=${DD_API_KEY}
      - DD_SITE=${DD_SITE}
      - DD_LOGS_ENABLED=true
    volumes:
      - /var/run/docker.sock:/var/run/docker.sock
      - /proc/:/host/proc:ro
      - /sys/fs/cgroup/:/host/sys/fs/cgroup:ro
```

This is an example docker-compose file that defines one service that runs the datadog agent alongside two other services. The datadog container is based on the image gcr.io/datadoghq/agent:7, which is the official datadog agent image.
The service-1 and service-2 services are both labeled with the com.datadoghq.ad.logs label. This label is used to specify the logs that the Datadog agent should collect. In this case, the com.datadoghq.ad.logs label is set to '[{"source": "docker", "service": "service-1"}]' for the service-1 service, and '[{"source": "docker", "service": "service-2"}]' for the service-2 service. This tells the Datadog agent to collect logs from the service-1 and service-2 services, respectively.

The pid property is set to host, which means that the container will share the host's PID namespace. This is useful for monitoring processes running on the host, as it allows the Datadog agent to see the host's processes as if they were running inside the container.

The environment property is used to set environment variables for the container. In this case, three environment variables are set: DD_API_KEY, DD_SITE, and DD_LOGS_ENABLED. The DD_API_KEY and DD_SITE variables are used to authenticate the Datadog agent with your Datadog account. The DD_LOGS_ENABLED variable is used to enable log collection in the Datadog agent.

The volumes property is used to mount host directories or files as volumes in the container. In this case, three volumes are mounted: /var/run/docker.sock, /proc/, and /sys/fs/cgroup/. The /var/run/docker.sock volume is used to allow the Datadog agent to collect logs from other containers running on the host. The /proc/ and /sys/fs/cgroup/ volumes are used to allow the Datadog agent to collect metrics from the host's processes and system.
