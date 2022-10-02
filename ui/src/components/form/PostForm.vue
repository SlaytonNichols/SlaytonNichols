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
        :model-value="props.modelValue?.name" 
        @input="updateName">
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

const nameFormVal = reactive({
  // getter
  get() {
    return props.modelValue?.name
  },
  // setter
  set(newValue: string) {    
    props.modelValue.name = newValue    
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
