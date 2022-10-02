<template>
  <div>
    <markdown-page 
      :frontmatter="frontmatterValue" 
      @edit="editPost"
      @create="savePost"
      @save="savePost"
      :allow-edit="admin"
      :is-edit-mode="isEditMode.get() && admin"
      :is-create-mode="isCreateMode.get() && admin">
      <div 
        v-if="!isEditMode.get() && !isCreateMode.get()" 
        v-html="renderedMdText.get()" 
        class="markdown-body">
      </div>
      <div v-else>
        <text-input
          :id="post.name" 
          :model-value="nameFormVal.get()" 
          @input="updateName">
        </text-input>
        <text-input
          :id="post.path" 
          :model-value="pathFormVal.get()" 
          @input="updatePath">
        </text-input>
        <text-area-input
          :id="post.mdText" 
          :model-value="mdTextFormVal.get()" 
          @input="updateMdText">
        </text-area-input>
      </div>      
    </markdown-page>    
  </div>
</template>

<script setup lang="ts">
import { onMounted, ref, reactive } from "vue"
import { QueryPosts, UpdatePost, CreatePost, Post } from "@/dtos"
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
const isAdmin = ref<Boolean>()

const admin = reactive({
  get () {
    return auth.value.roles.indexOf('Admin') >= 0
  }
})

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

const idFormVal = reactive({
  // getter
  get() {
    return post.value.id
  },
  // setter
  set(newValue: string) {    
    post.value.id = newValue    
  }
})

const mdTextFormVal = reactive({
  // getter
  get() {
    return post.value.mdText
  },
  // setter
  set(newValue: string) {    
    post.value.mdText = newValue    
  }
})

const nameFormVal = reactive({
  // getter
  get() {
    return post.value.name
  },
  // setter
  set(newValue: string) {    
    post.value.name = newValue    
  }
})

const pathFormVal = reactive({
  // getter
  get() {
    return post.value.path
  },
  // setter
  set(newValue: string) {    
    post.value.path = newValue    
  }
})

const updateMdText = async ($event) => {  
  mdTextFormVal.set($event.target.value)
}

const updateName = async ($event) => {  
  nameFormVal.set($event.target.value)
}

const updatePath = async ($event) => {  
  pathFormVal.set($event.target.value)
}

const updateId = async ($event) => {  
  idFormVal.set($event.target.value)
}

const getPost = async () => {    
  const api = await client.api(new QueryPosts())  
  totalPosts.set(Math.max.apply(null, api.response.results.map(x => x.id)) + 1)
  console.log(totalPosts.get())
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
  let request = new CreatePost({
    id: post.value.id,
    mdText: post.value.mdText,
    name: post.value.name,
    path: post.value.path
  })

  await client.api(request) 
  router.push({path: `/posts/${post.value.path}`})
}

const editPost = async () => {  
  isEditMode.set(!isEditMode.get())
  if(!isEditMode.get()) {
    rawMdText.set(mdTextFormVal.get())
    var md = new marked()
    var renderedMd = md.render(mdTextFormVal.get())
    renderedMdText.set(renderedMd)
  } else if (isEditMode.get()) {    
    mdTextFormVal.set(rawMdText.get())
  }  
  let request = new UpdatePost(
  {  
    id: post.value.id,
    mdText: post.value.mdText,
    name: post.value.name,
    path: post.value.path
  })
  await client.api(request)
  
  router.push({path: `/posts/${post.value.path}`})
}

onMounted(async () => {  
  await getPost()
  isEditMode.set(false)  
  if (router.currentRoute.value.params.Post === 'create') {    
    isCreateMode.set(true)    
    currentPost.set({ id: totalPosts.get(), name: '', path: '', mdText: '' });    
  }  
})

</script>
