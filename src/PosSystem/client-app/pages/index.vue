<template>
  <div>
    <!-- This page will automatically redirect to dashboard -->
  </div>
</template>

<script setup lang="ts">
// Redirect to dashboard page
await navigateTo('/dashboard', { replace: true })
</script>