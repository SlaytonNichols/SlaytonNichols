<template>
  <div>      
    <text-input
        :id="'Id'"          
        :model-value="props.modelValue?.id" 
        @input="updateId"
        hidden>
    </text-input>
    <text-input
        :id="'Title'" 
        :placeholder="'Post Title'"
        :model-value="props.modelValue?.title" 
        @input="updateTitle">
    </text-input>
    <text-input
        :id="'Summary'" 
        :placeholder="'Summary of Post'"
        :model-value="props.modelValue?.summary" 
        @input="updateSummary">
    </text-input>
    <text-input
        :id="'Path'" 
        :placeholder="'/posts/{path}'"
        :model-value="props.modelValue?.path" 
        @input="updatePath">
    </text-input>
    <text-area-input
        :id="'MarkdownBody'" 
        :placeholder="'## Markdown Post'"
        :model-value="props.modelValue?.mdText" 
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

const props = defineProps<{ modelValue: Post }>()

const currentPost = reactive({
  // getter
  get() {
    return props.modelValue
  },
  // setter
  set(newValue: Post) {
    
    props.modelValue.value = newValue
  }
})

const idFormVal = reactive({
  // getter
  get() {
    return props.modelValue?.id
  },
  // setter
  set(newValue: number) {    
    props.modelValue.id = newValue    
  }
})

const mdTextFormVal = reactive({
  // getter
  get() {
    return props.modelValue?.mdText
  },
  // setter
  set(newValue: string) {    
    props.modelValue.mdText = newValue    
  }
})

const titleFormVal = reactive({
  // getter
  get() {
    return props.modelValue?.title
  },
  // setter
  set(newValue: string) {    
    props.modelValue.title = newValue    
  }
})

const pathFormVal = reactive({
  // getter
  get() {
    return props.modelValue?.path
  },
  // setter
  set(newValue: string) {    
    props.modelValue.path = newValue    
  }
})

const summaryFormVal = reactive({
  // getter
  get() {
    return props.modelValue?.summary
  },
  // setter
  set(newValue: string) {    
    props.modelValue.summary = newValue    
  }
})

const updateMdText = async ($event) => {  
  mdTextFormVal.set($event.target.value)
}

const updateTitle = async ($event) => {  
  titleFormVal.set($event.target.value)
}

const updatePath = async ($event) => {  
  pathFormVal.set($event.target.value)
}

const updateId = async ($event) => {  
  idFormVal.set($event.target.value)
}

const updateSummary = async ($event) => {  
  summaryFormVal.set($event.target.value)
}

onMounted(async () => {  
  
})

</script>
