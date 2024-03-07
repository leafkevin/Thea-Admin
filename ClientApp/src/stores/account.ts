import { ref, computed } from "vue";
import { defineStore } from "pinia";

export const useUserStore = defineStore(
  "user",
  () => {
    const userId = ref<string>();
    const userName = ref<string>();
    const accessToken = ref<string>();
    const refreshToken = ref<string>();
    const roles = ref<string>();
    const menuRoutes = ref<IMenuRoute[]>([]);
    const showMenuRoutes = ref<IMenuRoute[]>([]);
    const flatMenuRoutes = ref<IMenuRoute[]>([]);
    const breadcrumbs = ref<IMenuRoute[]>([]);
    const expiresTime = ref<number>(0);

    const isExpired = computed(() => expiresTime.value <= new Date().getTime() / 1000);
    const isAuthorized = computed(() => userId.value !== undefined);
    const hasMenuRoutes = computed(() => menuRoutes.value && menuRoutes.value.length > 0);

    function setState(state: IUserState) {
      userId.value = state.userId;
      userName.value = state.userName;
      accessToken.value = state.accessToken;
      refreshToken.value = state.refreshToken;
      roles.value = state.roles;
      expiresTime.value = state.expires;
      menuRoutes.value = state.menuRoutes;
      breadcrumbs.value = getBreadcrumbs(state.menuRoutes) as IMenuRoute[];
      showMenuRoutes.value = getShowMenuRoutes(state.menuRoutes);
      flatMenuRoutes.value = getFlatMenuRoutes(state.menuRoutes);
    }
    function clearState() {
      userId.value = undefined;
      userName.value = undefined;
      accessToken.value = undefined;
      refreshToken.value = undefined;
      roles.value = undefined;
      menuRoutes.value = [];
    }
    function setMenuRoutes(myMenuRoutes: IMenuRoute[]) {
      menuRoutes.value = myMenuRoutes;
    }
    function clearMenuRoutes() {
      menuRoutes.value = [];
    }
    /**
     * @description 使用递归过滤出需要渲染在左侧菜单的列表 (需剔除 isHidden == true 的菜单)
     * @param {Array} menuList 菜单列表
     * @returns {Array}
     * */
    function getShowMenuRoutes(menuRoutes: IMenuRoute[]) {
      let myMenuRoutes: IMenuRoute[] = JSON.parse(JSON.stringify(menuRoutes));
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
        result[item.path] = [...parent, item];
        if (item.children) getBreadcrumbs(item.children, result[item.path], result);
      }
      return result;
    }
    function getFlatMenuRoutes(menuRoutes: IMenuRoute[]): IMenuRoute[] {
      let myMenuRoutes: IMenuRoute[] = JSON.parse(JSON.stringify(menuRoutes));
      return myMenuRoutes.flatMap(item => [item, ...(item.children ? getFlatMenuRoutes(item.children) : [])]);
    }

    return {
      userId,
      userName,
      accessToken,
      refreshToken,
      roles,
      menuRoutes,
      showMenuRoutes,
      flatMenuRoutes,
      expiresTime,
      breadcrumbs,
      isAuthorized,
      isExpired,
      hasMenuRoutes,
      setState,
      clearState,
      setMenuRoutes,
      clearMenuRoutes
    };
  },
  {
    persist: true
  }
);
