<template>
  <div class="min-h-screen">
    <main class="flex justify-center">
      <div class="mx-auto px-5">
        <AppBreadcrumb class="my-8" name="Blog" />
        <div v-for="route in postRoutes" class="mb-8">
          <router-link class="text-2xl hover:text-green-600" :to="route.path">{{ route.frontmatter.title }}
          </router-link>
          <p v-if="route.frontmatter.summary" class="text-gray-500">
            {{ route.frontmatter.summary }}
          </p>
        </div>
      </div>
    </main>
  </div>
</template>

<script setup lang="ts">
import { useRouter } from "vue-router"

type FrontMatter = {
  title: string
  summary?: string
  date?: string
}

const router = useRouter()
const postRoutes = router.getRoutes()
  .filter(r => r.path.startsWith("/posts/") && r.meta?.frontmatter)
  .map(r => ({ path: r.path, name: r.name, frontmatter: (r.meta as any)?.frontmatter as FrontMatter }))
  .filter(r => !r.path.includes("employment-history"))
  .sort((a, b) => (b.frontmatter.date ?? "")?.localeCompare(a.frontmatter.date ?? ""))

</script>
