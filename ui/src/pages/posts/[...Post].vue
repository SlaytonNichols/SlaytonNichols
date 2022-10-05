<template>
  <div class="flex">
    <markdown-page 
      class="flex-grow"
      v-if="!isEditMode.get() && !isCreateMode.get()"
      :frontmatter="frontmatterValue.get()">
      <div         
        v-html="renderedMdText.get()" 
        class="markdown-body pt-4">
      </div>      
    </markdown-page>   
    <div v-else class="pt-4 flex-grow">
        <post-form
          :model-value="currentPost.get()"     
          @edit="editPost"
          @create="createPost"      
          @save="updatePost"
          @delete="deletePost"
          :allow-edit="admin"
          :is-edit-mode="isEditMode.get()"
          :is-create-mode="isCreateMode.get()"   
        />
    </div> 
    <div v-if="admin" class="mb-4 mt-4 mr-4">
      <button type="button" title="edit">
        <Edit @click="editPost" v-if="!isEditMode.get() && !isCreateMode.get()" />        
      </button>
    </div>
  </div>
</template>

<script setup lang="ts">
import { onMounted, ref, reactive } from "vue"
import { Post } from "@/dtos"
import marked from "markdown-it"
import { useAttrs } from 'vue'
import { useRouter } from "vue-router"
import { auth } from "@/auth"
import Edit from "~icons/ci/edit/"
import { usePostsStore } from "@/stores/posts"

const store = usePostsStore()
store.refreshPosts()

type FrontMatter = {
  title: string
  summary?: string
  date?: string
}
const frontmatter = ref<FrontMatter>()
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

const updatePost = async () => {  
  await store.updatePost(post.value)
  router.push(`/posts/${post.value.path}`)
  await exitEditState()
}

const editPost = async () => {  
  isEditMode.set(!isEditMode.get())  
  if(!isEditMode.get()) {    
    var md = new marked()
    var renderedMd = md.render(currentPost.get().mdText)
    renderedMdText.set(renderedMd)
  }
}

const createPost = async () => {  
  await store.addPost(currentPost.get())
  router.push(`/posts/${post.value.path}`)
  await exitEditState()
}

const deletePost = async () => {  
  await store.removePost(post.value.id)
  router.push('/posts')
  await exitEditState()
}

const exitEditState = async () => {  
  isEditMode.set(false)  
  isCreateMode.set(false)
}

onMounted(async () => {  
  await exitEditState()
  if (router.currentRoute.value.params.Post === 'create') {    
    isCreateMode.set(true)   
    currentPost.set({ id: 0, title: '', summary: '', path: '', mdText: '' });
  } else {
    currentPost.set(await store.getPost(attrs.Post));    
    var md = new marked()
    var renderedMd = md.render(currentPost.get().mdText)
    renderedMdText.set(renderedMd)
    frontmatterValue.set({ 
        title: currentPost.get().title, 
        summary: currentPost.get().summary
      })    
  }
})

</script>
