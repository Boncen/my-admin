<template>
  <router-view v-slot="{ Component, route }">
    <transition name="fade" mode="out-in" appear>
      <component
        :is="Component"
        v-if="route.meta.ignoreCache"
        :key="route.fullPath"
      />
      <keep-alive v-else :include="cacheList">
        <component v-if="Component" :is="Component" :key="route.fullPath" />
        <Home v-else />
      </keep-alive>
    </transition>
  </router-view>
</template>

<script lang="ts" setup>
  import { computed } from 'vue';
  import { useTabBarStore } from '@/store';
  import Home from '@/views/home/index.vue'

  const tabBarStore = useTabBarStore();

  const cacheList = computed(() => tabBarStore.getCacheList);
</script>

<style scoped lang="less"></style>
