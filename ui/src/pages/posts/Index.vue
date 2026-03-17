<template>
  <div class="min-h-screen">
    <main class="flex justify-center">
      <div class="px-5 max-w-4xl w-full mt-10 mb-16">
        <AppBreadcrumb class="my-4 justify-center" name="Blog" />

        <div class="flex flex-wrap justify-center gap-2 mb-8">
          <button
            class="px-3 py-1 rounded-full text-sm font-medium transition-colors"
            :class="selectedTag === null ? 'bg-indigo-600 text-white' : 'bg-gray-100 text-gray-600 hover:bg-gray-200 dark:bg-gray-800 dark:text-gray-400 dark:hover:bg-gray-700'"
            @click="selectedTag = null"
          >
            All
          </button>
          <button
            v-for="tag in availableTags"
            :key="tag"
            class="px-3 py-1 rounded-full text-sm font-medium transition-colors"
            :class="selectedTag === tag ? 'bg-indigo-600 text-white' : 'bg-gray-100 text-gray-600 hover:bg-gray-200 dark:bg-gray-800 dark:text-gray-400 dark:hover:bg-gray-700'"
            @click="toggleTag(tag)"
          >
            #{{ tag }}
          </button>
        </div>

        <div v-if="pinnedPosts.length" class="mb-12">
          <h2 class="text-xs font-semibold uppercase tracking-widest text-gray-400 dark:text-gray-500 mb-6 text-center">
            Pinned
          </h2>
          <div
            v-for="post in pinnedPosts"
            :key="post.path"
            class="flex mb-10 justify-center flex-col items-center text-center"
          >
            <router-link class="text-2xl hover:text-green-600 font-semibold" :to="post.path">
              {{ post.frontmatter.title }}
            </router-link>

            <div class="mt-2 flex flex-wrap items-center justify-center gap-2 text-sm">
              <span v-if="post.frontmatter.date" class="text-gray-400">
                {{ formatDate(post.frontmatter.date) }}
              </span>
              <span
                v-if="post.frontmatter.draft"
                class="px-2 py-0.5 rounded-full bg-amber-100 text-amber-700 dark:bg-amber-900/40 dark:text-amber-300"
              >
                Draft
              </span>
            </div>

            <p v-if="post.frontmatter.summary" class="text-gray-500 mt-2 max-w-2xl">
              {{ post.frontmatter.summary }}
            </p>

            <div v-if="post.frontmatter.tags?.length" class="mt-3 flex flex-wrap justify-center gap-2">
              <button
                v-for="tag in post.frontmatter.tags"
                :key="`${post.path}-${tag}`"
                class="text-xs px-2 py-1 rounded-full bg-gray-100 text-gray-500 hover:bg-gray-200 dark:bg-gray-800 dark:text-gray-400 dark:hover:bg-gray-700"
                @click="toggleTag(tag)"
              >
                #{{ tag }}
              </button>
            </div>
          </div>
        </div>

        <div
          v-for="post in unpinnedPosts"
          :key="post.path"
          class="flex mb-10 justify-center flex-col items-center text-center"
        >
          <router-link class="text-2xl hover:text-green-600 font-semibold" :to="post.path">
            {{ post.frontmatter.title }}
          </router-link>

          <div class="mt-2 flex flex-wrap items-center justify-center gap-2 text-sm">
            <span v-if="post.frontmatter.date" class="text-gray-400">
              {{ formatDate(post.frontmatter.date) }}
            </span>
            <span
              v-if="post.frontmatter.draft"
              class="px-2 py-0.5 rounded-full bg-amber-100 text-amber-700 dark:bg-amber-900/40 dark:text-amber-300"
            >
              Draft
            </span>
          </div>

          <p v-if="post.frontmatter.summary" class="text-gray-500 mt-2 max-w-2xl">
            {{ post.frontmatter.summary }}
          </p>

          <div v-if="post.frontmatter.tags?.length" class="mt-3 flex flex-wrap justify-center gap-2">
            <button
              v-for="tag in post.frontmatter.tags"
              :key="`${post.path}-${tag}`"
              class="text-xs px-2 py-1 rounded-full bg-gray-100 text-gray-500 hover:bg-gray-200 dark:bg-gray-800 dark:text-gray-400 dark:hover:bg-gray-700"
              @click="toggleTag(tag)"
            >
              #{{ tag }}
            </button>
          </div>
        </div>

        <div v-if="filteredPosts.length === 0" class="text-center text-gray-500 mt-10">
          No posts found for this tag.
        </div>
      </div>
    </main>
  </div>
</template>

<script setup lang="ts">
import { computed, ref } from "vue"
import { useRouter } from "vue-router"

type FrontMatter = {
  title: string
  summary?: string
  date?: string
  tags?: string[]
  draft?: boolean
  pinned?: boolean
  hidden?: boolean
}

type PostRoute = {
  path: string
  frontmatter: FrontMatter
}

const router = useRouter()
const selectedTag = ref<string | null>(null)

const allPosts = computed<PostRoute[]>(() => {
  return router.getRoutes()
    .filter(route => route.path.startsWith("/posts/") && route.meta?.frontmatter)
    .map(route => ({
      path: route.path,
      frontmatter: (route.meta as any).frontmatter as FrontMatter,
    }))
    .sort((a, b) => {
      const aDate = a.frontmatter.date ?? ""
      const bDate = b.frontmatter.date ?? ""
      return bDate.localeCompare(aDate) || a.path.localeCompare(b.path)
    })
})

const availableTags = computed(() => {
  const publishedPosts = allPosts.value.filter(post => !post.frontmatter.draft && !post.frontmatter.hidden)
  return Array.from(new Set(publishedPosts.flatMap(post => post.frontmatter.tags ?? []))).sort()
})

const filteredPosts = computed(() => {
  let posts = allPosts.value.filter(post => !post.frontmatter.draft && !post.frontmatter.hidden)

  if (selectedTag.value) {
    posts = posts.filter(post => (post.frontmatter.tags ?? []).includes(selectedTag.value!))
  }

  return posts
})

const pinnedPosts = computed(() => filteredPosts.value.filter(post => post.frontmatter.pinned))
const unpinnedPosts = computed(() => filteredPosts.value.filter(post => !post.frontmatter.pinned))

const toggleTag = (tag: string) => {
  selectedTag.value = selectedTag.value === tag ? null : tag
}

const formatDate = (value: string) => {
  const date = new Date(value)
  if (Number.isNaN(date.getTime())) return value

  return date.toLocaleDateString("en-US", {
    year: "numeric",
    month: "short",
    day: "numeric",
    timeZone: "UTC",
  })
}
</script>
