<script setup lang="ts">
import { useApp } from "@/api"
import { datadogRum } from '@datadog/browser-rum';

// https://github.com/vueuse/head
// you can use this to manipulate the document head in any components,
// they will be rendered correctly in the html results with vite-ssg
useHead({
  title: 'SlaytonNichols',
  meta: [
    { name: 'description', content: 'Vite SSG Starter Template' },
  ],
})

if (!import.meta.env.SSR) {
  let store = useApp()
  store.load()
  datadogRum.init({
    applicationId: '6f4b27ea-61b6-4676-a1c0-96492230da6e',
    clientToken: 'pubd496da5f7fd8ca374c30db223be9a4a9',
    site: 'us3.datadoghq.com',
    service:'slaytonnichols',
    
    // Specify a version number to identify the deployed version of your application in Datadog 
    // version: '1.0.0',
    sampleRate: 100,
    sessionReplaySampleRate: 20,
    trackInteractions: true,
    trackResources: true,
    trackLongTasks: true,
    defaultPrivacyLevel:'mask-user-input'
});
    
datadogRum.startSessionReplayRecording();
}
</script>

<template>
  <router-view />
</template>
