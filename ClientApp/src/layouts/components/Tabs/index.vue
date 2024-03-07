<template>
  <div class="tabs-box">
    <div class="tabs-menu">
      <el-tabs v-model="tabPagePath" type="card" @tab-click="tabPageClick" @tab-remove="removeTagPages">
        <el-tab-pane v-for="item in tabPages" :key="item.path" :label="item.title" :name="item.path" :closable="item.close">
          <template #label>
            <el-icon v-if="item.icon && tabsIcon" class="tabs-icon">
              <component :is="item.icon"></component>
            </el-icon>
            {{ item.title }}
          </template>
        </el-tab-pane>
      </el-tabs>
      <MoreButton />
    </div>
  </div>
</template>

<script setup lang="ts">
  import Sortable from "sortablejs";
  import { ref, computed, watch, onMounted } from "vue";
  import { useRoute, useRouter } from "vue-router";
  import { useGlobalStore } from "@/stores/global";
  import { useTabPageStore } from "@/stores/tabPages";
  import { useUserStore } from "@/stores/account";
  import { TabsPaneContext, TabPaneName } from "element-plus";
  import MoreButton from "./components/MoreButton.vue";

  const route = useRoute();
  const router = useRouter();
  const tabPageStore = useTabPageStore();
  const userStore = useUserStore();
  const globalStore = useGlobalStore();

  const tabPagePath = ref(route.fullPath);
  const tabPages = computed(() => tabPageStore.tabPages);
  const tabsIcon = computed(() => globalStore.tabsIcon);

  onMounted(() => {
    tabPagesDrop();
    initTabPages();
  });

  // 监听路由的变化（防止浏览器后退/前进不变化 tabPagePath）
  watch(
    () => route.fullPath,
    () => {
      if (route.meta.isFull) return;
      tabPagePath.value = route.fullPath;
      const tabsParams = {
        icon: route.meta.icon as string,
        title: route.meta.title as string,
        path: route.fullPath,
        name: route.name as string,
        close: !route.meta.isAffix,
        isKeepAlive: route.meta.isKeepAlive as boolean
      };
      tabPageStore.addTabPage(tabsParams);
    },
    { immediate: true }
  );

  // 初始化需要固定的 tabs
  const initTabPages = () => {
    userStore.flatMenuRoutes.forEach(item => {
      if (item.meta.isAffix && !item.meta.isHidden && !item.meta.isFull) {
        const tabsParams = {
          icon: item.meta.icon,
          title: item.meta.title,
          path: item.path,
          name: item.name,
          close: !item.meta.isAffix,
          isKeepAlive: item.meta.isKeepAlive
        };
        tabPageStore.addTabPage(tabsParams);
      }
    });
  };

  // tabs 拖拽排序
  const tabPagesDrop = () => {
    Sortable.create(document.querySelector(".el-tabs__nav") as HTMLElement, {
      draggable: ".el-tabs__item",
      animation: 300,
      onEnd({ newIndex, oldIndex }) {
        const myTabPages = [...tabPageStore.tabPages];
        const currRow = myTabPages.splice(oldIndex as number, 1)[0];
        myTabPages.splice(newIndex as number, 0, currRow);
        tabPageStore.setTabPages(myTabPages);
      }
    });
  };

  // Tab Click
  const tabPageClick = (tabItem: TabsPaneContext) => {
    const fullPath = tabItem.props.name as string;
    router.push(fullPath);
  };

  // Remove Tab
  const removeTagPages = (fullPath: TabPaneName) => {
    tabPageStore.removeTabPage(fullPath as string, fullPath == route.fullPath);
  };
</script>

<style scoped lang="scss">
  @import "./index.scss";
</style>
@/stores/tabPages
