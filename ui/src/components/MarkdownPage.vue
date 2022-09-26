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

type FrontMatter = {
  title: string
}
defineProps<{ frontmatter: FrontMatter }>()

</script>
