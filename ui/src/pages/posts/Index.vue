<template>
  <div class="min-h-screen">
    <main class="flex">
      <div class="px-5 flex-grow mt-10">
        <AppBreadcrumb class="my-4 justify-center" name="Blog" />
        <form-loading
          class="justify-center"
          v-if="loading.get()"
          :loading="loading.get()"
          :icon="true"
          text=""
        />
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
          <Add class="w-8 h-8" />
        </button>      
      </div>
    </main>
  </div>
</template>

<script setup lang="ts">
import { useRouter } from "vue-router"
import { onMounted, reactive, ref } from "vue"
import { Post } from "@/dtos"
import Add from "~icons/bxs/add-to-queue/"
import { auth } from "@/auth"
import { usePostsStore } from "@/stores/posts"

const store = usePostsStore()

type FrontMatter = {
  title: string
  summary?: string
  date?: string
}
const isAdmin = auth?.value?.roles.indexOf('Admin') >= 0
const posts = ref<Post[]>([])
const isLoading = ref<Boolean>()
const loading = reactive({
  // getter
  get() {
    return isLoading.value
  },
  // setter
  set(newValue: Boolean) {
    
    isLoading.value = newValue
  }
})
const router = useRouter()  
posts.value = router.getRoutes()
  .filter(r => r.path.startsWith("/posts/") && r.meta?.frontmatter)
  .map(r => ({ path: r.path, name: r.name, frontmatter: (r.meta as any)?.frontmatter as FrontMatter }))
  .filter(r => !r.path.includes("employment-history"))
  .sort((a, b) => (b.frontmatter.date ?? "")?.localeCompare(a.frontmatter.date ?? ""))

const createPost = async () => {
  router.push({path: '/posts/create'})
}

onMounted(async () => {  
  loading.set(true)
  await store.refreshPosts()  
  store.allPosts.forEach(result => {
    posts.value.unshift({ 
      id: result.id,
      path: '/posts/' + result.path, 
      title: result.title, 
      draft: result.draft,
      frontmatter: { 
        title: result.title, 
        summary: result.summary
      } 
    })
  });

  //place favorite posts at the top, TODO: replace with tags
  let favorites = ["todos"]  
  let favoritPosts = posts.value.filter(x => favorites.includes(x.path));
  let otherPosts = posts.value.filter(x => !favorites.includes(x.path));
  posts.value = favoritPosts.concat(otherPosts)

  if(!isAdmin) {
    posts.value = posts.value.filter(x => !x.draft)
  }  
  loading.set(false)
})

</script>
<style scoped>
.align-center {
  align-items: center;
}
</style>