<template>
  <header class="border-b border-gray-200 pr-3">
    <div class="flex flex-wrap items-center">
      <div class="flex-shrink flex-grow-0">
        <router-link to="/">
          <div class="p-2 cursor-pointer">
            <img class="w-13 h-13" src="https://user-images.githubusercontent.com/45402324/88486759-ff3f6e80-cf4d-11ea-8869-cb0de304b698.png" alt="SlaytonNichols logo" />
          </div>
        </router-link>
      </div>
      <div class="flex flex-grow flex-shrink flex-nowrap justify-end items-center">
        <nav class="relative flex flex-grow">
          <ul class="flex flex-wrap items-center justify-end w-full">
            <NavItem>
              <NavLink class="text-2xl" href="/posts">Blog</NavLink>
            </NavItem>
            <NavItem>
              <a class="text-2xl hover:text-green-600 m-1" href="https://linkedin.com/in/slaytonnichols">LinkedIn</a>
            </NavItem>
            <NavItem>
              <a class="text-2xl hover:text-green-600 m-1" href="https://github.com/SlaytonNichols">Github</a>
            </NavItem>
          </ul>
        </nav>
      </div>
    </div>
  </header>
</template>

<script setup lang="ts">
import NavItem from "@/components/NavItem.vue";
import NavLink from "@/components/NavLink.vue";
</script>
