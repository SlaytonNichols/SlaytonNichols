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
// TODO: Add prop to allow this to be hidden on some md pages
// import AppBreadcrumb from './AppBreadcrumb.vue'

type FrontMatter = {
  title: string
}
defineProps<{ frontmatter: FrontMatter }>()

</script>
