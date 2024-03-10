import { ref } from "vue";
import router from "@/routers";
import { defineStore } from "pinia";
import { getUrlWithParams } from "@/utils";
import { useKeepAliveStore } from "./keepAlive";
import { ITabPageState } from "./types";

const keepAliveStore = useKeepAliveStore();

export const useTabPageStore = defineStore(
  "tabPages",
  () => {
    //tabPages
    const tabPages = ref<ITabPageState[]>([]);

    //addTabs
    function addTabPage(tabItem: ITabPageState) {
      if (tabPages.value.every(item => item.path !== tabItem.path)) {
        tabPages.value.push(tabItem);
      }
      // add keepalive
      if (!keepAliveStore.keepAliveNames.includes(tabItem.name) && tabItem.isKeepAlive) {
        keepAliveStore.addKeepAliveName(tabItem.path);
      }
    }
    // Remove Tabs
    function removeTabPage(tabPath: string, isCurrent: boolean = true) {
      if (isCurrent) {
        tabPages.value.forEach((item, index) => {
          if (item.path !== tabPath) return;
          const nextTab = tabPages.value[index + 1] || tabPages.value[index - 1];
          if (!nextTab) return;
          router.push(nextTab.path);
        });
      }
      // remove keepalive
      const tabItem = tabPages.value.find(item => item.path === tabPath);
      tabItem?.isKeepAlive && keepAliveStore.removeKeepAliveName(tabItem.path);
      // set tabs
      tabPages.value = tabPages.value.filter(item => item.path !== tabPath);
    }
    // Close Tabs On Side
    function closeTabPageOnSide(path: string, type: "left" | "right") {
      const currentIndex = tabPages.value.findIndex(item => item.path === path);
      if (currentIndex !== -1) {
        const range = type === "left" ? [0, currentIndex] : [currentIndex + 1, tabPages.value.length];
        tabPages.value = tabPages.value.filter((item, index) => {
          return index < range[0] || index >= range[1] || !item.close;
        });
      }
      // set keepalive
      const KeepAliveList = tabPages.value.filter(item => item.isKeepAlive);
      keepAliveStore.setKeepAliveNames(KeepAliveList.map(item => item.path));
    }
    // Close MultipleTab
    function closeTabPages(tabsMenuValue?: string) {
      tabPages.value = tabPages.value.filter(item => {
        return item.path === tabsMenuValue || !item.close;
      });
      // set keepalive
      const KeepAliveList = tabPages.value.filter(item => item.isKeepAlive);
      keepAliveStore.setKeepAliveNames(KeepAliveList.map(item => item.path));
    }
    // Set Tabs
    function setTabPages(myTabPages: ITabPageState[]) {
      tabPages.value = myTabPages;
    }
    // Set Tabs Title
    function setTabPageTitle(title: string) {
      tabPages.value.forEach(item => {
        if (item.path == getUrlWithParams()) item.title = title;
      });
    }

    return {
      tabPages,
      addTabPage,
      removeTabPage,
      closeTabPageOnSide,
      closeTabPages,
      setTabPages,
      setTabPageTitle
    };
  },
  {
    persist: true
  }
);
