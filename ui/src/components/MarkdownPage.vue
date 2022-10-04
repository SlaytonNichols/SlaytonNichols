<template>
  <div class="min-h-screen">
    <main class="flex justify-center">
      <div class="px-5">
        <article class="prose lg:prose-xl">
          <slot></slot>
        </article>
      </div>
    </main>
  </div>
</template>

<script setup lang="ts">
import { reactive, ref } from "vue"

type FrontMatter = {
  title: string
  summary?: string
  date?: string
}

const props = defineProps<{
  frontmatter?: FrontMatter|null
}>()

</script>
<style scoped>
.flex-basis {
  flex-basis: 100%;
}
</style>
