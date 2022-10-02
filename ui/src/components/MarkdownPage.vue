<template>
  <div class="min-h-screen">
    <main class="flex justify-center">
      <div class="px-5 flex-basis">
        <article class="prose lg:prose-xl">
          <slot></slot>
        </article>
      </div>      
    </main>
    <div v-if="allowEdit" class="flex justify-end mr-4 mb-4">
      <button>
        <Edit @click="$emit('edit')" v-if="!props.isEditMode && !props.isCreateMode" />
        <Add @click="$emit('create')" v-else-if="props.isCreateMode"/>
        <Save @click="$emit('save')" v-else/>
      </button>
    </div>    
  </div>
</template>

<script setup lang="ts">
// TODO: Add prop to allow this to be hidden on some md pages
// import AppBreadcrumb from './AppBreadcrumb.vue'

import { reactive, ref } from "vue"
import Edit from "~icons/ci/edit/"
import Save from "~icons/fluent/save-20-filled/"
import Add from "~icons/bxs/add-to-queue/"

type FrontMatter = {
  title: string
  summary?: string
  date?: string
}

const props = defineProps<{
  frontmatter?: FrontMatter|null
  allowEdit: boolean|false
  label?: string
  isEditMode: boolean|false
  isCreateMode: boolean|false
}>()

</script>
<style scoped>
.flex-basis {
  flex-basis: 50%;
}
</style>
