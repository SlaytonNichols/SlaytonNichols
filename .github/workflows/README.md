# GitHub Actions

`release.yml` builds the static site container, pushes it to GHCR, then deploys it to the DigitalOcean droplet over SSH.

## Required secrets

- `DEPLOY_HOST` - SSH host for the droplet
- `DEPLOY_USERNAME` - SSH user on the droplet
- `DEPLOY_KEY` - private key used for SSH deploys
- `LETSENCRYPT_EMAIL` - email used by nginx-proxy / Let's Encrypt
- `DEPLOY_API` - public hostname for the site
