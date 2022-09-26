<template>
  <div>
    <router-link v-if="props.useRouter" :to="$attrs.href" class="flex items-center justify-start mw-full p-4 hover:text-green-600" v-bind="$attrs">
      <slot></slot>
    </router-link>
    <a v-if="!props.useRouter" :to="$attrs.href" class="flex items-center justify-start mw-full p-4 hover:text-green-600" v-bind="$attrs">
      <slot></slot>
    </a>
  </div>
</template>

<script setup lang="ts">

import { attrs } from "@/auth"
import { computed } from "vue";

const props = defineProps<{
  useRouter?: boolean
}>()

</script>
