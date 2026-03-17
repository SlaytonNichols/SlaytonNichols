# Handoff Message (for Discord / OpenClaw)

Repo: `~/Developer/SlaytonNichols` (https://github.com/SlaytonNichols/SlaytonNichols)

Read `PLAN.md` in the repo root for full context. Summary:

**Goal**: Migrate slaytonnichols.com from a full-stack C#/.NET + Vue + MongoDB blog with auth into a **simple static markdown blog**. No API, no database, no auth. Just markdown files rendered by a static site generator, served via nginx in Docker on a DO droplet.

**Key tasks (in order)**:
1. Replace the Vue + .NET stack with a static site generator (Astro recommended — already in Node/Vite ecosystem)
2. Rewrite the home page with new "what I do" language (full text in PLAN.md)
3. Migrate ALL existing blog posts (both markdown files and DB-stored posts) to flat markdown files with frontmatter — preserve all current content
4. Add tag filtering on the blog index page
5. Delete the `api/` directory and all auth pages
6. Rewrite the Dockerfile to just serve static files via nginx
7. Simplify the GitHub Actions workflow and docker-compose — remove .NET build, MongoDB, Datadog
8. Keep the existing layout/colors and the DO Docker SSH deployment flow

**Constraints**: Keep same visual design. Keep DO droplet + Docker + GitHub Actions deploy. Markdown only, no DB. No auth.

Everything is documented in `PLAN.md`.
