<template>
  <div>      
    <text-input
        :id="post.id" 
        :model-value="idFormVal.get()" 
        @input="updateId"
        hidden>
    </text-input>
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

const post = ref<Post>()
const postsCount = ref<number>()
const router = useRouter()

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
  set(newValue: number) {    
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

onMounted(async () => {  
  
})

</script>
