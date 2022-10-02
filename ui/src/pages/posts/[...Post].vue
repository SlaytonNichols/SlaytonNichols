<template>
  <div>
    <markdown-page 
      :frontmatter="frontmatterValue.get()" 
      @edit="editPost"
      @create="createPost"      
      @save="savePost"
      @delete="deletePost"
      :allow-edit="admin"
      :is-edit-mode="isEditMode.get()"
      :is-create-mode="isCreateMode.get()">
      <div 
        v-if="!isEditMode.get() && !isCreateMode.get()" 
        v-html="renderedMdText.get()" 
        class="markdown-body pt-4">
      </div>
      <div v-else class="pt-4">
        <post-form          
          :model-value="currentPost.get()"
          @update="onUpdate"
        />
      </div>      
    </markdown-page>    
  </div>
</template>

<script setup lang="ts">
import { onMounted, ref, reactive } from "vue"
import { QueryPosts, UpdatePost, CreatePost, DeletePost, Post } from "@/dtos"
import { client } from "@/api"
import marked from "markdown-it"
import { useAttrs } from 'vue'
import { useRouter } from "vue-router"
import { auth } from "@/auth"

type FrontMatter = {
  title: string
  summary?: string
  date?: string
}
const frontmatter = ref<FrontMatter>()
const mdText = ref<string>()
const mdHtml = ref<string>()
const editMode = ref<Boolean>()
const createMode = ref<Boolean>()
const attrs = useAttrs()
const post = ref<Post>()
const postsCount = ref<number>()
const router = useRouter()
const admin = auth?.value?.roles.indexOf('Admin') >= 0

const isEditMode = reactive({
  // getter
  get() {
    return editMode.value
  },
  // setter
  set(newValue: Boolean) {
    
    editMode.value = newValue
  }
})

const isCreateMode = reactive({
  // getter
  get() {
    return createMode.value
  },
  // setter
  set(newValue: Boolean) {
    
    createMode.value = newValue
  }
})

const currentPost = reactive({
  // getter
  get() {
    return post.value
  },
  // setter
  set(newValue: Post) {
    
    post.value = newValue
  }
})

const renderedMdText = reactive({
  // getter
  get() {
    return mdHtml.value
  },
  // setter
  set(newValue: string) {
    
    mdHtml.value = newValue
  }
})

const rawMdText = reactive({
  // getter
  get() {
    return mdText.value
  },
  // setter
  set(newValue: string) {
    
    mdText.value = newValue
  }
})

const frontmatterValue = reactive({
  // getter
  get() {
    return frontmatter.value
  },
  // setter
  set(newValue: FrontMatter) {    
    frontmatter.value = newValue
  }
})

const totalPosts = reactive({
  // getter
  get() {
    return postsCount.value
  },
  // setter
  set(newValue: number) {    
    postsCount.value = newValue
  }
})

const onUpdate = async ($event) => {  
  console.log($event.target.value)
}

const getPost = async () => {    
  const api = await client.api(new QueryPosts())  
  totalPosts.set(Math.max.apply(null, api.response.results.map(x => x.id)))  
  let result = api.response.results.filter(p => p.path === attrs.Post)[0]
  if(api.succeeded && result){
    var md = new marked()
    renderedMdText.set(md.render(result.mdText))      
    rawMdText.set(result.mdText)
    frontmatterValue.set({ 
        title: result.name, 
        summary: 'Add a summary property'
      })
    currentPost.set(result);
  }
      
  return currentPost.get()
}

const savePost = async () => {  
  let request = new UpdatePost(
  {  
    id: post.value.id,
    mdText: post.value.mdText,
    name: post.value.name,
    path: post.value.path
  })
  await client.api(request)
  isEditMode.set(!isEditMode.get())
  router.go(0)
}

const editPost = async () => {  
  await getPost()
  isEditMode.set(!isEditMode.get())
  //if turning edit mode off or loading the page
  if(!isEditMode.get()) {
    rawMdText.set(currentPost.get().mdText)
    var md = new marked()
    var renderedMd = md.render(currentPost.get().mdText)
    renderedMdText.set(renderedMd)
  }
  currentPost.set({
    id: post.value.id,
    mdText: post.value.mdText,
    name: post.value.name,
    path: post.value.path
  })
}

const createPost = async () => {  
  let request = new CreatePost({
    id: totalPosts.get() + 1,
    mdText: post.value.mdText,
    name: post.value.name,
    path: post.value.path
  })

  await client.api(request) 
  isEditMode.set(false)  
  isCreateMode.set(false)
  router.push(`/posts/${post.value.path}`)
}

const deletePost = async () => {  
  let request = new DeletePost({
    id: post.value.id
  })

  await client.api(request) 
  isEditMode.set(false)  
  isCreateMode.set(false)
  router.push('/posts')
}

onMounted(async () => {  
  await getPost()
  isEditMode.set(false)  
  isCreateMode.set(false)
  if (router.currentRoute.value.params.Post === 'create') {    
    isCreateMode.set(true)   
    currentPost.set({ id: totalPosts.get(), name: '', path: '', mdText: '' });    
  }  
})

</script>
