## Deployment Configuration

- DO Droplet

  - Install docker-compose

  ```bash
  sudo curl -L "https://github.com/docker/compose/releases/download/1.29.2/docker-compose-$(uname -s)-$(uname -m)" -o /usr/local/bin/docker-compose
  sudo chmod +x /usr/local/bin/docker-compose
  docker-compose --version
  ```

  - Install Docker

  ```bash
  sudo apt update
  sudo apt install apt-transport-https ca-certificates curl software-properties-common
  curl -fsSL https://download.docker.com/linux/ubuntu/gpg | sudo apt-key add -
  sudo add-apt-repository "deb [arch=amd64] https://download.docker.com/linux/ubuntu focal stable"
  apt-cache policy docker-ce
  sudo apt install docker-ce
  ```

  - Copy nginx-proxy-compose to root directory

  ```bash
  docker-compose -f nginx-proxy-compose.yml up -d
  ```

- DO DNS Management

  - nicholsslayton.com -> Github Pages IPs
  - nicholsslayton.com. -> DO nameservers
  - api.nicholsslayton.com. -> DO Reserved IP

- DO PostgreSQL DB
