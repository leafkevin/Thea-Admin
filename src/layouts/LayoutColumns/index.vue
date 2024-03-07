<!-- 分栏布局 -->
<template>
  <el-container class="layout">
    <div class="aside-split">
      <div class="logo flx-center">
        <img class="logo-img" src="@/assets/images/logo.svg" alt="logo" />
      </div>
      <el-scrollbar>
        <div class="split-list">
          <div
            v-for="item in menuRoutes"
            :key="item.path"
            class="split-item"
            :class="{
              'split-active': splitActive === item.path || `/${splitActive.split('/')[1]}` === item.path
            }"
            @click="changeSubMenu(item)">
            <el-icon>
              <component :is="item.meta.icon"></component>
            </el-icon>
            <span class="title">{{ item.meta.title }}</span>
          </div>
        </div>
      </el-scrollbar>
    </div>
    <el-aside :class="{ 'not-aside': !subMenuList.length }" :style="{ width: isCollapse ? '65px' : '210px' }">
      <div class="logo flx-center">
        <span v-show="subMenuList.length" class="logo-text">{{ isCollapse ? "G" : title }}</span>
      </div>
      <el-scrollbar>
        <el-menu
          :router="false"
          :default-active="activeMenu"
          :collapse="isCollapse"
          :unique-opened="accordion"
          :collapse-transition="false">
          <SubMenu :menu-list="subMenuList" />
        </el-menu>
      </el-scrollbar>
    </el-aside>
    <el-container>
      <el-header>
        <ToolBarLeft />
        <ToolBarRight />
      </el-header>
      <Main />
    </el-container>
  </el-container>
</template>

<script setup lang="ts" name="layoutColumns">
  import { ref, computed, watch } from "vue";
  import { useRoute, useRouter } from "vue-router";
  import { useUserStore } from "@/stores/account";
  import { useGlobalStore } from "@/stores/global";
  import Main from "@/layouts/components/Main/index.vue";
  import ToolBarLeft from "@/layouts/components/Header/ToolBarLeft.vue";
  import ToolBarRight from "@/layouts/components/Header/ToolBarRight.vue";
  import SubMenu from "@/layouts/components/Menu/SubMenu.vue";

  const title = import.meta.env.VITE_GLOB_APP_TITLE;

  const route = useRoute();
  const router = useRouter();
  const userStore = useUserStore();
  const globalStore = useGlobalStore();
  const accordion = computed(() => globalStore.accordion);
  const isCollapse = computed(() => globalStore.isCollapse);
  const menuRoutes = computed(() => userStore.menuRoutes);
  const activeMenu = computed(() => (route.meta.menuPath ? route.meta.menuPath : route.path) as string);

  const subMenuList = ref<IMenuRoute[]>([]);
  const splitActive = ref("");
  watch(
    () => [menuRoutes, route],
    () => {
      // 当前菜单没有数据直接 return
      if (!menuRoutes.value || !menuRoutes.value.length) return;
      splitActive.value = route.path;
      const menuItem = menuRoutes.value.filter((item: IMenuRoute) => {
        return route.path === item.path || `/${route.path.split("/")[1]}` === item.path;
      });
      if (menuItem[0].children?.length) return (subMenuList.value = menuItem[0].children);
      subMenuList.value = [];
    },
    {
      deep: true,
      immediate: true
    }
  );

  // change SubMenu
  const changeSubMenu = (item: IMenuRoute) => {
    splitActive.value = item.path;
    if (item.children?.length) return (subMenuList.value = item.children);
    subMenuList.value = [];
    router.push(item.path);
  };
</script>

<style scoped lang="scss">
  @import "./index.scss";
</style>
