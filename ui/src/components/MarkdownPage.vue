<template>
  <div class="min-h-screen">
    <main class="flex justify-center">
      <div :class="(!props.isEditMode && !props.isCreateMode ? 'px-5' : 'px-5 flex-basis')">
        <article :class="(!props.isEditMode && !props.isCreateMode ? 'prose lg:prose-xl' : 'lg:prose-xl')">
          <slot></slot>
        </article>
      </div>      
      <div v-if="allowEdit ?? false" class="mr-4 mb-4 mt-8">
        <button>
          <Edit class="mb-4 mt-4" @click="$emit('edit')" v-if="!props.isEditMode && !props.isCreateMode" />
          <Add class="mb-4 mt-4" @click="$emit('create')" v-else-if="props.isCreateMode"/>
          <Save class="mb-4 mt-4" @click="$emit('save')" v-else-if="props.isEditMode"/>          
        </button>
        <button v-if="props.isEditMode">
          <Delete class="mb-4 mt-4" @click="$emit('delete')"/>
        </button>
      </div>
    </main>
  </div>
</template>

<script setup lang="ts">
// TODO: Add prop to allow this to be hidden on some md pages
// import AppBreadcrumb from './AppBreadcrumb.vue'

import { reactive, ref } from "vue"
import Edit from "~icons/ci/edit/"
import Save from "~icons/fluent/save-20-filled/"
import Add from "~icons/bxs/add-to-queue/"
import Delete from "~icons/fluent/delete-20-filled/"

type FrontMatter = {
  title: string
  summary?: string
  date?: string
}

const props = defineProps<{
  frontmatter?: FrontMatter|null
  allowEdit?: boolean|false
  label?: string
  isEditMode?: boolean|false
  isCreateMode?: boolean|false
}>()

</script>
<style scoped>
.flex-basis {
  flex-basis: 100%;
}
</style>
