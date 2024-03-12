import { ref, computed } from "vue";
import { defineStore } from "pinia";
import { IMenuRoute } from "./types";
import router from "@/routers";
import { RouteRecordRaw } from "vue-router";

const modules = import.meta.glob("@/views/**/*.vue");
export const useMenuStore = defineStore("menu", () => {
  const menuRoutes = ref<IMenuRoute[]>([]);
  const breadcrumbs = ref<{ [key: string]: any }>([]);

  const hasMenuRoutes = computed(() => menuRoutes.value && menuRoutes.value.length > 0);
  const flatMenuRoutes = computed(() => getFlatMenuRoutes(menuRoutes.value));
  const showMenuRoutes = computed(() => getShowMenuRoutes(menuRoutes.value));

  function setState(myMenuRoutes: IMenuRoute[]) {
    menuRoutes.value = myMenuRoutes;
    breadcrumbs.value = getBreadcrumbs(myMenuRoutes);
  }
  function clearState() {
    menuRoutes.value = [];
  }
  function loadAsyncMenuRoutes() {
    flatMenuRoutes.value.map(f => {
      f.component = modules["/src/views" + f.component + ".vue"];
      if (f.meta?.isFull) {
        router.addRoute(f as unknown as RouteRecordRaw);
      } else {
        router.addRoute("Layout", f as unknown as RouteRecordRaw);
      }
    });
  }
  /**
   * @description 使用递归过滤出需要渲染在左侧菜单的列表 (需剔除 isHidden == true 的菜单)
   * @param {Array} menuList 菜单列表
   * @returns {Array}
   * */
  function getShowMenuRoutes(menuRoutes: IMenuRoute[]) {
    let myMenuRoutes = JSON.parse(JSON.stringify(menuRoutes));
    return myMenuRoutes.filter(item => {
      item.children?.length && (item.children = getShowMenuRoutes(item.children));
      return !item.meta?.isHidden;
    });
  }
  /**
   * @description 使用递归找出所有面包屑存储到 pinia/vuex 中
   * @param {Array} menuRoutes 菜单列表
   * @param {Array} parent 父级菜单
   * @param {Object} result 处理后的结果
   * @returns {Object}
   */
  function getBreadcrumbs(menuRoutes: IMenuRoute[], parent = [], result: { [key: string]: any } = {}) {
    for (const item of menuRoutes) {
      if (item.meta.routeType > 1) {
        if (item.meta.routeType == 2) {
          result[item.path] = [...parent, item];
          getBreadcrumbs(item.children!, result[item.path], result);
        } else result[item.path] = parent;
      } else {
        result[item.path] = [...parent, item];
        if (item.children && item.children?.length > 0) getBreadcrumbs(item.children, result[item.path], result);
      }
    }
    return result;
  }
  function getFlatMenuRoutes(menuRoutes: IMenuRoute[]): IMenuRoute[] {
    let myMenuRoutes: IMenuRoute[] = JSON.parse(JSON.stringify(menuRoutes));
    return myMenuRoutes.flatMap(item => [item, ...(item.children ? getFlatMenuRoutes(item.children) : [])]);
  }

  return {
    menuRoutes,
    showMenuRoutes,
    flatMenuRoutes,
    breadcrumbs,
    hasMenuRoutes,
    loadAsyncMenuRoutes,
    setState,
    clearState
  };
});
