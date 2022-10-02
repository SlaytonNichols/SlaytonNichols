<template>
  <div>      
    <text-input
        :id="'Id'"          
        :model-value="idFormVal.get()" 
        @input="updateId"
        hidden>
    </text-input>
    <text-input
        :id="'Title'" 
        :placeholder="'Post Title'"
        :model-value="nameFormVal.get()" 
        @input="updateName">
    </text-input>
    <text-input
        :id="'Path'" 
        :placeholder="'/posts/{path}'"
        :model-value="pathFormVal.get()" 
        @input="updatePath">
    </text-input>
    <text-area-input
        :id="'MarkdownBody'" 
        :placeholder="'## Markdown Post'"
        :model-value="mdTextFormVal.get()" 
        @input="updateMdText">
    </text-area-input>
  </div>
</template>

<script setup lang="ts">
import { onMounted, ref, reactive } from "vue"
import { Post } from "@/dtos"

type FrontMatter = {
  title: string
  summary?: string
  date?: string
}

const modelValue = ref<Post>()
const postsCount = ref<number>()

const currentPost = reactive({
  // getter
  get() {
    return modelValue.value
  },
  // setter
  set(newValue: Post) {
    
    modelValue.value = newValue
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
    return modelValue?.value?.id
  },
  // setter
  set(newValue: number) {    
    modelValue.value.id = newValue    
  }
})

const mdTextFormVal = reactive({
  // getter
  get() {
    return modelValue?.value?.mdText
  },
  // setter
  set(newValue: string) {    
    modelValue.value.mdText = newValue    
  }
})

const nameFormVal = reactive({
  // getter
  get() {
    return modelValue?.value?.name
  },
  // setter
  set(newValue: string) {    
    modelValue.value.name = newValue    
  }
})

const pathFormVal = reactive({
  // getter
  get() {
    return modelValue?.value?.path
  },
  // setter
  set(newValue: string) {    
    modelValue.value.path = newValue    
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
//   console.log(idFormVal.get())
})

</script>
