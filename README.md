[![Release](https://github.com/SlaytonNichols/SlaytonNichols/actions/workflows/release.yml/badge.svg)](https://github.com/SlaytonNichols/SlaytonNichols/actions/workflows/release.yml)
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
![SlaytonNicholsLogo](https://user-images.githubusercontent.com/45402324/88486759-ff3f6e80-cf4d-11ea-8869-cb0de304b698.png)

## Who am I?

My name is Slayton Nichols. I grew up in a small part of Southern Appalachia — Western North Carolina and the North Georgia Mountains. I care about music, software, and the outdoors. Read more [here](https://nicholsslayton.com).

## What is this Repo?

This repo now houses the source for a static markdown-driven personal site and blog. The goal is to keep publishing simple: write posts in markdown, build a static site, ship it in Docker.

## Local Development

```bash
cd ui
npm install
npm run dev
```

## Production Build

```bash
cd ui
npm run build
```

The release workflow builds the static site, packages it into an nginx image, pushes to GHCR, then deploys it to the DigitalOcean droplet over SSH.

## Pages

- [Home](https://nicholsslayton.com)
- [Employment History](https://nicholsslayton.com/posts/employment-history)
- [Blog](https://nicholsslayton.com/posts)
- [Currently Working On](https://nicholsslayton.com/posts/todos)
