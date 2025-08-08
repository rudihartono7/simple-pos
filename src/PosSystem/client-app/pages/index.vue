<template>
  <div>
    <!-- This page will automatically redirect to dashboard -->
  </div>
</template>

<script setup lang="ts">
// Only redirect on client-side to prevent server-side redirect loops
if (process.client) {
  await navigateTo('/dashboard', { replace: true })
}
</script>