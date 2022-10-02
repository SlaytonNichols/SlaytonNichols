<template>
  <div class="min-h-screen">
    <main class="flex justify-center">
      <div class="px-5 flex-basis">
        <article class="prose lg:prose-xl">
          <slot></slot>
        </article>
      </div>      
    </main>
    <div class="flex justify-end mr-4">
      <button @click="$emit('edit')">Edit/Save</button>
    </div>    
  </div>
</template>

<script setup lang="ts">
// TODO: Add prop to allow this to be hidden on some md pages
// import AppBreadcrumb from './AppBreadcrumb.vue'

import { reactive, ref } from "vue"

type FrontMatter = {
  title: string
  summary?: string
  date?: string
}
const frontmatter = ref<FrontMatter>()
const allowEdit = ref<Boolean>()

const isEdit = reactive({
  // getter
  get() {
    return allowEdit.value
  },
  // setter
  set(newValue: boolean) {    
    allowEdit.value = newValue
  }
})

</script>
<style scoped>
.flex-basis {
  flex-basis: 50%;
}
</style>
