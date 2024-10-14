<template>
    <div class="container">
      <!-- <Breadcrumb :items="['menu.list', 'menu.list.searchTable']" /> -->
      <TablePage></TablePage>
    </div>
  </template>
  
  <script lang="ts" setup>
  import TablePage from '@/components/tablePage/index.vue'
  
   
  </script>
  
  <script lang="ts">
    export default {
      name: 'users',
    };
  </script>
  
  <style scoped lang="less">
    .container {
      padding: 10px 10px 10px 10px;
    }
  </style>
  