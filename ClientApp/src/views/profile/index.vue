<template>
  <div class="card content-box">
    <el-tabs :tab-position="tabPosition" class="tabs">
      <el-tab-pane label="基本信息" :stretch="true">
       
      </el-tab-pane>
      <el-tab-pane label="安全设置">安全设置</el-tab-pane>
    </el-tabs>
  </div>
</template>
<script lang="ts" setup>
  import { ref } from "vue";

  const tabPosition = ref("left");
</script>
<style>
  .tabs {
    width: 100%;
    .tabs > .el-tabs__content {
      padding: 32px;

      /* font-size: 32px;
      font-weight: 600; */
      color: #6b778c;
    }
    .el-tabs__nav {
      width: 180px;
    }
    .el-tabs--right .el-tabs__content,
    .el-tabs--left .el-tabs__content {
      height: 100%;
    }
  }
</style>
