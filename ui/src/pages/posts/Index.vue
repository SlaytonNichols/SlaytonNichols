<template>
  <div class="min-h-screen">
    <main class="flex">
      <div class="px-5 flex-grow mt-10">
        <AppBreadcrumb class="my-4 justify-center" name="Blog" />
        <div v-for="route in posts" class="flex mb-8 justify-center flex-col align-center">
          <router-link class="text-2xl hover:text-green-600" :to="route.path">{{ route.frontmatter.title }}
          </router-link>
          <p v-if="route.frontmatter.summary" class="text-gray-500">
            {{ route.frontmatter.summary }}
          </p>
        </div>
      </div>
      <div v-if="isAdmin" class="m-4 justify-end">
        <button @click="createPost">
          <Add />
        </button>      
      </div>
    </main>
  </div>
</template>

<script setup lang="ts">
import { useRouter } from "vue-router"
import { onMounted, ref } from "vue"
import { Post, QueryPosts } from "@/dtos"
import { client } from "@/api"
import Add from "~icons/bxs/add-to-queue/"
import { auth } from "@/auth"

type FrontMatter = {
  title: string
  summary?: string
  date?: string
}
const isAdmin = auth?.value?.roles.indexOf('Admin') >= 0
const posts = ref<Post[]>([])
const router = useRouter()  
posts.value = router.getRoutes()
  .filter(r => r.path.startsWith("/posts/") && r.meta?.frontmatter)
  .map(r => ({ path: r.path, name: r.name, frontmatter: (r.meta as any)?.frontmatter as FrontMatter }))
  .filter(r => !r.path.includes("employment-history"))
  .sort((a, b) => (b.frontmatter.date ?? "")?.localeCompare(a.frontmatter.date ?? ""))

const createPost = async () => {
  router.push({path: '/posts/create'})
}

const refreshPosts = async () => {
  let apiRoutes = []
  const api = await client.api(new QueryPosts())
    
  if(api.succeeded && api.response!.results){    
    apiRoutes = api.response.results.forEach(result => {
      posts.value.push({ 
        id: result.id,
        path: '/posts/' + result.path, 
        title: result.title, 
        frontmatter: { 
          title: result.title, 
          summary: result.summary
        } 
      })
    });
  }
  return apiRoutes  
}

onMounted(async () => {  
  await refreshPosts()  
})

</script>
<style scoped>
.align-center {
  align-items: center;
}
</style>