# Website Redesign Plan — slaytonnichols.com

## Overview

Simplify the current website from a full-stack C#/.NET + Vue + MongoDB blog with auth into a **static markdown blog** served from a Docker container on a DigitalOcean droplet. Keep the existing layout, colors, and deployment pipeline (GitHub Actions → Docker → SSH to DO droplet). Remove all accounts/auth, the .NET API, and the MongoDB dependency.

---

## Current Architecture (What We Have)

- **Frontend**: Vue 3 + Vite + Tailwind CSS (in `ui/`)
- **Backend**: C# .NET 6 ServiceStack API with MongoDB for storing blog posts (in `api/`)
- **Blog posts**: Mix of local `.md` files (via vite-plugin-pages frontmatter) AND posts stored in MongoDB fetched via API
- **Auth**: ServiceStack auth with admin roles, sign-in/sign-up pages
- **Deployment**: GitHub Actions → build UI → build .NET Docker image → push to GHCR → SSH into DO droplet → `docker compose up`
- **Extras**: Datadog APM tracing, nginx-proxy with Let's Encrypt

## Target Architecture (What We Want)

- **Frontend**: Static site generator (e.g., Astro, Hugo, or simplified Vite + markdown plugin) — whatever is simplest
- **Blog posts**: **ONLY markdown files** in the repo, no database
- **No backend API** — no .NET, no MongoDB, no ServiceStack
- **No auth** — no sign-in, sign-up, admin pages
- **Deployment**: Keep the same GitHub Actions → Docker → SSH to DO droplet flow, but the Docker image just serves static files (nginx or similar)
- **Keep**: Current layout structure, color scheme, nav header/footer style
- **Add**: Tags/filters on blog posts (via frontmatter metadata)

---

## Updated Home Page Language

Replace the current tech-list-focused home page with language that emphasizes **what I do and how I think**, not just what tools I use. The new home page content should read something like this:

```markdown
## Software Engineer

### Who am I?

My name is Slayton Nichols. I grew up in a small part of Southern Appalachia — Western North Carolina and the North Georgia Mountains. I'm passionate about music, building software, and the outdoors.

### What I Do

I build and maintain software systems, with a focus on solving ambiguous problems end-to-end. My work spans backend services, data platforms, and internal tools — I care most about shipping reliable software and making complex systems easier to reason about.

**Problem Decomposition & Systematic Debugging**
- Own ambiguous production issues end-to-end: reproduce failures, form hypotheses, validate root causes through data analysis, and iterate on solutions
- Build mental models for tracing data flow through distributed systems to isolate where transformations diverge from expected behavior
- Develop repeatable debugging workflows and document investigation patterns for team knowledge sharing

**Cross-Functional Collaboration & Requirements Translation**
- Partner with product and QA to translate vague business requirements into testable technical specifications
- Lead technical implementation of multi-sprint efforts coordinating across teams, managing scope creep, and communicating blockers early
- Bridge the gap between business domain experts and engineering by learning domain language and documenting edge cases

**Data Quality & Validation-First Development**
- Apply defensive programming patterns: null handling, boundary conditions, graceful degradation when upstream data is malformed
- Build validation and testing into development workflow rather than treating it as an afterthought
- Champion observability by adding structured logging and metrics to surface data quality issues before they reach production

**Iterative Refinement & Technical Debt Management**
- Refactor legacy code incrementally, extracting shared patterns without blocking feature work
- Balance shipping pressure with sustainable architecture decisions
- Maintain backwards compatibility during migrations by running old and new paths in parallel for validation

### What is this Website?

This site is a cross between a resume, a blog, and a notepad. I use it to document projects, write up experiments, and share what I'm learning.
```

---

## TODOs (Ordered by Priority)

### Phase 1 — Migrate to Simple Markdown Blog

1. **Choose a static site generator** — Astro or Hugo recommended. Astro is the natural fit since we're already in a Node/Vite ecosystem and it has first-class markdown support with frontmatter. Hugo is faster but requires learning Go templates.
2. **Scaffold new site in `ui/`** (or replace `ui/` entirely)
   - Recreate the current layout: NavHeader, NavFooter, page structure
   - Match existing Tailwind-based color scheme and typography
   - Home page (`index.md`) renders the new language above
   - Blog index page lists all markdown posts with title, summary, date
   - Individual post pages render markdown content
3. **Migrate ALL existing content**
   - Keep `employment-history.md` and all other existing markdown posts
   - Export all DB-stored posts from MongoDB and convert them to markdown files
   - Place everything in a `posts/` or `content/` directory
   - Each post gets frontmatter: `title`, `summary`, `date`, `tags`, `draft`
   - **Do not discard any existing blog posts** — all current content must be preserved
4. **Add tags/filters**
   - Tags defined in frontmatter (e.g., `tags: [trading, ml, devops]`)
   - Blog index page supports filtering by tag
5. **Remove the .NET API entirely** — delete `api/` directory
6. **Remove auth pages** — delete sign-in, sign-up, admin, profile pages
7. **Update Dockerfile** — replace the .NET Docker image with a simple nginx container serving the static build output
8. **Update GitHub Actions** — simplify `release.yml`:
   - Remove .NET build steps
   - Build static site
   - Build nginx Docker image
   - Push to GHCR
   - SSH deploy to DO droplet (same flow, just simpler image)
9. **Update `docker-compose-template.yml`** — remove MongoDB, Datadog, and .NET environment variables; just serve static files on port 80/443 behind nginx-proxy with Let's Encrypt
10. **Remove Datadog APM** — not needed for a static site

### Phase 2 — New Blog Posts

11. **Write blog post: AI Income Journey** — document what we've been doing with AI tools, trading bots, etc.
12. **Write blog post: Trading Bot Journey** — document the hyro-trader project, what worked, what didn't
13. **Write blog post: Basic ML Projects** — write up any ML experiments, feature importance analysis, labeling strategies, etc.
14. **Write blog post: Site Migration** — meta-post about migrating this blog from a full-stack app to a static markdown site

---

## Deployment Architecture (Keep This)

The deployment flow stays the same, just simplified:

```
Push to main → GitHub Actions → Build static site → Build Docker image (nginx + static files) → Push to GHCR → SSH into DO droplet → docker compose pull && up -d
```

The DO droplet already runs nginx-proxy + Let's Encrypt companion containers. The new site container just needs to:
- Expose port 80
- Set `VIRTUAL_HOST` and `LETSENCRYPT_HOST` environment variables
- Serve static files from nginx

### Current GitHub Secrets Needed (Keep)
- `DEPLOY_HOST`, `DEPLOY_USERNAME`, `DEPLOY_KEY` — SSH into DO droplet
- `LETSENCRYPT_EMAIL` — for cert generation
- `DEPLOY_API` — the domain name

### GitHub Secrets to Remove
- `DEFAULTCONNECTION`, `MONGO`, `AUTHKEYBASE64` — no more database
- `DD_API_KEY`, `DD_SITE` — no more Datadog

---

## Key Files Reference

| Current File | Action |
|---|---|
| `ui/` | Replace with static site (Astro or similar) |
| `api/` | **Delete entirely** |
| `Dockerfile` | Rewrite — nginx serving static files |
| `.deploy/docker-compose-template.yml` | Simplify — remove DB, Datadog |
| `.github/workflows/release.yml` | Simplify — remove .NET build steps |
| `.deploy/ci.prebuild.sh` | Likely no longer needed |
| `ui/src/pages/index.md` | Rewrite with new language |
| `ui/src/pages/posts/` | Replace with flat markdown files |
| `service.datadog.yaml` | **Delete** |

---

## Constraints

- **Keep the same visual layout and colors** — this is a simplification of the backend, not a redesign
- **Keep the DO Docker deployment** — SSH-based deploy to droplet via GitHub Actions
- **Markdown files only** — no database, no CMS, no API
- **No auth** — public read-only site
