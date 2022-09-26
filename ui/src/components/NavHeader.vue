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
              <NavLink :use-router="true" class="text-lg" href="/posts">
                <Markdown class="m-2"/> Blog
              </NavLink>
            </NavItem>
            <NavItem>
              <NavLink :use-router="false" class="text-lg " href="https://linkedin.com/in/slaytonnichols">
                <Linkedin class="m-2"/> Linkedin
              </NavLink>
            </NavItem>
            <NavItem>
              <NavLink :use-router="false" class="text-lg " href="https://github.com/SlaytonNichols">
                <Github class="m-2"/> Github
              </NavLink>
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
import Linkedin from '~icons/logos/linkedin-icon'
import Github from '~icons/logos/github-icon'
import Markdown from '~icons/cib/markdown/'
</script>
