<template>
<div>
    <button @click="editPost">Edit</button>
    <markdown-page v-if="!isEditMode.get()" :frontmatter="frontmatterValue">
      <div v-html="renderedMdText.get()" class="markdown-body"></div>
    </markdown-page>
    <text-area-input :id="'Test'" v-else-if="isEditMode.get()" :model-value="inputValue.get()" @input="updatePostText"></text-area-input>    
  </div>
</template>

<script setup lang="ts">
import { onMounted, ref, reactive } from "vue"
import { QueryPosts } from "@/dtos"
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

const refreshPosts = async () => {  
  let apiRoutes = []
  let request = new QueryPosts()
  request.name = attrs.Post
  const api = await client.api(request)
    
  if(api.succeeded && api.response!.results){    
    apiRoutes = api.response.results.forEach(result => {
      var md = new marked()
      renderedMdText.set(md.render(result.mdText))      
      rawMdText.set(result.mdText)
      frontmatterValue.set({ 
          title: result.name, 
          summary: 'Test' // TODO: replace this
        })
    });
  }
      
  return apiRoutes  
}

const updatePostText = async ($event) => {  
  inputValue.set($event.target.value)
}

const editPost = async () => {  
  isEditMode.set(!isEditMode.get())
  if(!isEditMode.get()) {
    rawMdText.set(inputValue.get())    
    var md = new marked()
    var renderedMd = md.render(rawMdText.get())
    renderedMdText.set(renderedMd)    
  } else if (isEditMode.get()) {    
    inputValue.set(rawMdText.get())
  }
}

onMounted(async () => {  
  isEditMode.set(false)
  await refreshPosts()  
})

</script>
