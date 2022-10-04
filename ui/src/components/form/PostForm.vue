<template>
  <api-form class="flex flex-col ml-4 mb-4" v-if="props.isEditMode || props.isCreateMode">
    <div class="flex-align-self-end">
      <button title="delete" v-if="props.isEditMode" type="submit" class="mr-2">
        <Delete @click="$emit('delete')"/>
      </button>
      <button title="submit" type="submit">
        <Add @click="$emit('create');" v-if="props.isCreateMode"/>
        <Save @click="$emit('save')" v-else-if="props.isEditMode"/>
      </button>
    </div>
    <div>
      <text-input        
        :id="'Title'" 
        :placeholder="'Post Title'"
        :model-value="props.modelValue?.title" 
        @input="updateTitle">
      </text-input>
      <text-input        
        :id="'Path'" 
        :placeholder="'/posts/{path}'"
        :model-value="props.modelValue?.path" 
        @input="updatePath">
      </text-input>
    </div>
    <div class="">
      <text-input
          :id="'Id'"          
          :model-value="props.modelValue?.id" 
          @input="updateId"
          hidden>
      </text-input>    
      <text-input
          :id="'Summary'" 
          :placeholder="'Summary of Post'"
          :model-value="props.modelValue?.summary" 
          @input="updateSummary">
      </text-input>
      <text-area-input
          :id="'MarkdownBody'" 
          :placeholder="'## Markdown Post'"
          :model-value="props.modelValue?.mdText" 
          @input="updateMdText">
      </text-area-input>
    </div>
  </api-form>
</template>

<script setup lang="ts">
import { onMounted, ref, reactive } from "vue"
import { Post } from "@/dtos"
import Save from "~icons/fluent/save-20-filled/"
import Add from "~icons/bxs/add-to-queue/"
import Delete from "~icons/fluent/delete-20-filled/"

type FrontMatter = {
  title: string
  summary?: string
  date?: string
}

const props = defineProps<{
  modelValue: Post
  frontmatter?: FrontMatter|null
  allowEdit?: boolean|false
  label?: string
  isEditMode?: boolean|false
  isCreateMode?: boolean|false
}>()

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
<style scoped>
.flex-basis {
  flex-basis: 100%;
}
.flex-align-self-end {
  align-self: flex-end;
}
</style>