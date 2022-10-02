<template>
  <div>
    <markdown-page 
      :frontmatter="frontmatterValue" 
      @edit="editPost">
      <div v-if="!isEditMode.get()" v-html="renderedMdText.get()" class="markdown-body"></div>
      <text-area-input class="markdown-body" v-else-if="isEditMode.get()" 
        :id="post.name" 
        :model-value="inputValue.get()" 
        @input="updatePostText">
      </text-area-input>
    </markdown-page>    
  </div>
</template>

<script setup lang="ts">
import { onMounted, ref, reactive } from "vue"
import { QueryPosts, UpdatePost, Post } from "@/dtos"
import { client } from "@/api"
import marked from "markdown-it"
import { useAttrs } from 'vue'

type FrontMatter = {
  title: string
  summary?: string
  date?: string
}
const frontmatter = ref<FrontMatter>()
const mdText = ref<string>()
const mdHtml = ref<string>()
const input = ref<string>('')
const editMode = ref<Boolean>()
const attrs = useAttrs()
const post = ref<Post>()

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

const inputValue = reactive({
  // getter
  get() {
    return input.value
  },
  // setter
  set(newValue: string) {    
    input.value = newValue
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

const refreshPost = async () => {  
  let request = new QueryPosts()
  request.name = attrs.Post
  const api = await client.api(request)
    
  if(api.succeeded && api.response!.results){    
    let result = api.response.results[0];
    var md = new marked()
    renderedMdText.set(md.render(result.mdText))      
    rawMdText.set(result.mdText)
    frontmatterValue.set({ 
        title: result.name, 
        summary: 'Test' // TODO: replace this
      })
    currentPost.set(api.response.results[0]);
  }
      
  return currentPost.get()  
}

const updatePostText = async ($event) => {  
  inputValue.set($event.target.value)
}

const savePost = async (rawMdText, post) => {
  let request = new UpdatePost(
    {  
      id: post.id,
      mdText: rawMdText,
      name: post.name,
      path: post.path
    })
  request.name = attrs.Post
  await client.api(request)  
}

const editPost = async () => {  
  isEditMode.set(!isEditMode.get())
  if(!isEditMode.get()) {
    rawMdText.set(inputValue.get())    
    var md = new marked()
    var renderedMd = md.render(rawMdText.get())
    renderedMdText.set(renderedMd)    
    savePost(rawMdText.get(), currentPost.get())

  } else if (isEditMode.get()) {    
    inputValue.set(rawMdText.get())
  }
}

onMounted(async () => {  
  isEditMode.set(false)
  await refreshPost()
})

</script>
