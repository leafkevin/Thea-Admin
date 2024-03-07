<template>
  <el-dropdown trigger="click" :teleported="false">
    <div class="more-button">
      <i :class="'iconfont icon-xiala'"></i>
    </div>
    <template #dropdown>
      <el-dropdown-menu>
        <el-dropdown-item @click="refresh">
          <el-icon><Refresh /></el-icon>{{ $t("tabs.refresh") }}
        </el-dropdown-item>
        <el-dropdown-item @click="maximize">
          <el-icon><FullScreen /></el-icon>{{ $t("tabs.maximize") }}
        </el-dropdown-item>
        <el-dropdown-item divided @click="closeCurrentTabPage">
          <el-icon><Remove /></el-icon>{{ $t("tabs.closeCurrent") }}
        </el-dropdown-item>
        <el-dropdown-item @click="tabPageStore.closeTabPageOnSide(route.fullPath, 'left')">
          <el-icon><DArrowLeft /></el-icon>{{ $t("tabs.closeLeft") }}
        </el-dropdown-item>
        <el-dropdown-item @click="tabPageStore.closeTabPageOnSide(route.fullPath, 'right')">
          <el-icon><DArrowRight /></el-icon>{{ $t("tabs.closeRight") }}
        </el-dropdown-item>
        <el-dropdown-item divided @click="tabPageStore.closeTabPages(route.fullPath)">
          <el-icon><CircleClose /></el-icon>{{ $t("tabs.closeOther") }}
        </el-dropdown-item>
        <el-dropdown-item @click="closeAllTabPages">
          <el-icon><FolderDelete /></el-icon>{{ $t("tabs.closeAll") }}
        </el-dropdown-item>
      </el-dropdown-menu>
    </template>
  </el-dropdown>
</template>

<script setup lang="ts">
  import { inject, nextTick } from "vue";
  import { HOME_URL } from "@/config";
  import { useTabPageStore } from "@/stores/tabPages";
  import { useGlobalStore } from "@/stores/global";
  import { useKeepAliveStore } from "@/stores/keepAlive";
  import { useRoute, useRouter } from "vue-router";

  const route = useRoute();
  const router = useRouter();
  const tabPageStore = useTabPageStore();
  const globalStore = useGlobalStore();
  const keepAliveStore = useKeepAliveStore();

  // refresh current page
  const refreshCurrentPage: Function = inject("refresh") as Function;
  const refresh = () => {
    setTimeout(() => {
      route.meta.isKeepAlive && keepAliveStore.removeKeepAliveName(route.name as string);
      refreshCurrentPage(false);
      nextTick(() => {
        route.meta.isKeepAlive && keepAliveStore.addKeepAliveName(route.name as string);
        refreshCurrentPage(true);
      });
    }, 0);
  };

  // maximize current page
  const maximize = () => {
    globalStore.setGlobalState("maximize", true);
  };

  // Close Current
  const closeCurrentTabPage = () => {
    if (route.meta.isAffix) return;
    tabPageStore.removeTabPage(route.fullPath);
  };

  // Close All
  const closeAllTabPages = () => {
    tabPageStore.closeTabPages();
    router.push(HOME_URL);
  };
</script>

<style scoped lang="scss">
  @import "../index.scss";
</style>
