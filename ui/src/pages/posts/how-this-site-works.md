---
title: "How This Site Works"
summary: "Vue SSG, Docker multi-stage builds, GitHub Actions, and a DigitalOcean droplet — the full stack behind nicholsslayton.com."
date: 2026-03-20
tags:
  - devops
  - vue
  - docker
  - infrastructure
draft: false
---

## How This Site Works

This site is a static markdown-driven blog built with Vue 3 and Vite SSG. Pages are markdown files with YAML frontmatter. The build compiles everything to static HTML, packages it in a Docker container, and deploys to a DigitalOcean droplet — all triggered by a push to `master`.

---

### The Stack

**Vue 3 + Vite SSG.** Pages live as `.md` files in `src/pages/`. A Vite plugin parses frontmatter at build time and injects it into route metadata — that's what drives the blog index, tag filtering, draft/hidden visibility, and pinned posts. Markdown is rendered as Vue components, which means I can embed Vue components inside blog posts if needed.

**Tailwind CSS + @tailwindcss/typography.** The `prose` class handles all markdown rendering styles. Dark mode is class-based, toggled by a script in the HTML head that checks `prefers-color-scheme` before the page paints — no flash of wrong theme.

**Static generation.** `vite-ssg build` pre-renders every route to static HTML at build time. The output is a plain `dist/` folder — no Node server required at runtime. Pages are interactive (Vue hydrates on the client), but the initial load is pure HTML.

---

### Build & Deploy

The whole pipeline runs in a single GitHub Actions workflow triggered on push to `master`:

**1. Build.** Install deps, run `vite-ssg build`, output static files.

**2. Containerize.** Multi-stage Dockerfile:

```dockerfile
FROM node:18-alpine AS build
WORKDIR /app
COPY ui/package*.json ./ui/
WORKDIR /app/ui
RUN npm install
COPY ui/ .
RUN npm run build

FROM nginx:stable-alpine
COPY nginx.conf /etc/nginx/conf.d/default.conf
COPY --from=build /app/ui/dist /usr/share/nginx/html
```

The final image is just nginx + static files. No Node runtime in production.

**3. Push to GHCR.** The image goes to GitHub Container Registry, tagged with the release version or `latest`.

**4. Deploy via SSH.** The workflow SCPs a docker-compose file to the droplet and runs `docker-compose up`. An nginx-proxy container handles routing and Let's Encrypt handles TLS certificates automatically.

---

### Routing

nginx is configured with a single rule that makes clean URLs work without trailing slashes:

```nginx
rewrite ^/(.+)/$ /$1 permanent;

location / {
  try_files $uri $uri.html $uri/index.html =404;
}
```

A request to `/posts/heart-rate-dlt` resolves to `heart-rate-dlt.html`. No client-side routing fallback needed — every page is a real file.

---

### Content Model

Blog posts are controlled entirely by frontmatter:

- `draft: true` — excluded from the blog index, not linked anywhere
- `hidden: true` — accessible by direct URL, excluded from listing
- `pinned: true` — promoted to the top of the blog index
- `tags` — drive the tag filter UI on `/posts`

The blog index component reads route metadata at runtime and filters accordingly. No build-time content pipeline, no CMS — just files and frontmatter.

---

### What I'd Change

The site works well for what it is, but there are rough edges:

- **No per-page SEO metadata.** Every page shares the same `<title>` and `<meta description>` unless I manually override in frontmatter. Easy to add, just hasn't been prioritized.
- **No sitemap generation.** Would be a straightforward build-time addition.
- **Dependency versions are pinned to 2022-era packages.** Vue 3.2, Vite 3, Tailwind 3. Everything works, but a major version bump is coming eventually.

The tradeoff is intentional: this is a side project, and I'd rather spend time on content than infrastructure. The site builds, deploys, and serves reliably — that's the bar.
