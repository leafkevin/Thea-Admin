import { createRouter, createWebHashHistory, createWebHistory } from "vue-router";
import { getMyRoutes, refreshToken } from "@/api/account";
import { useUserStore } from "@/stores/account";
import { useMenuStore } from "@/stores/menu";
import { LOGIN_URL, ROUTER_WHITE_LIST, SWITCH_ROLE_URL } from "@/config";
import { staticRouter } from "@/routers/staticRouter";
import NProgress from "@/utils/nprogress";
import { ElNotification } from "element-plus";
import { IMenuRoute, IUserState } from "@/stores/types";

const mode = import.meta.env.VITE_ROUTER_MODE;

const routerMode = {
  hash: () => createWebHashHistory(),
  history: () => createWebHistory()
};

/**
 * @description 📚 路由参数配置简介
 * @param path ==> 路由菜单访问路径
 * @param name ==> 路由 name (对应页面组件 name, 可用作 KeepAlive 缓存标识 && 按钮权限筛选)
 * @param redirect ==> 路由重定向地址
 * @param component ==> 视图文件路径
 * @param meta ==> 路由菜单元信息
 * @param meta.icon ==> 菜单和面包屑对应的图标
 * @param meta.title ==> 路由标题 (用作 document.title || 菜单的名称)
 * @param meta.activeMenu ==> 当前路由为详情页时，需要高亮的菜单
 * @param meta.linkUrl ==> 路由外链时填写的访问地址
 * @param meta.isHidden ==> 是否在菜单中隐藏 (通常列表详情页需要隐藏)
 * @param meta.isFull ==> 菜单是否全屏 (示例：数据大屏页面)
 * @param meta.isAffix ==> 菜单是否固定在标签页中 (首页通常是固定项)
 * @param meta.isKeepAlive ==> 当前路由是否缓存
 * */
const router = createRouter({
  history: routerMode[mode](),
  routes: [...staticRouter],
  strict: false,
  scrollBehavior: () => ({ left: 0, top: 0 })
});

/**
 * @description 路由拦截 beforeEach
 * */
router.beforeEach(async to => {
  const userStore = useUserStore();
  // 1.NProgress 开始
  NProgress.start();

  // 2.动态设置标题
  const title = import.meta.env.VITE_GLOB_APP_TITLE;
  document.title = to.meta.title ? `${to.meta.title} - ${title}` : title;

  // 3.判断是否登陆页面，直接放行
  if (to.path == LOGIN_URL) {
    userStore.clearState();
    resetRouter();
    return true;
  }

  // 4.判断访问页面是否在路由白名单地址(静态路由)中，如果存在直接放行
  if (ROUTER_WHITE_LIST.includes(to.path)) return true;

  // 5.判断是否有 Token，没有重定向到 login 页面
  if (!userStore.isAuthorized) return { path: LOGIN_URL, replace: true };

  // token已过期，调用刷新token接口
  if (userStore.isExpired) {
    const resp = await refreshToken({ refreshToken: userStore.refreshToken });
    const response = resp.data;
    if (response.isSuccess) {
      userStore.setState(response.data as IUserState);
      //code=9 跳转到切换角色页面
      if (response.code > 0) return { path: SWITCH_ROLE_URL, replace: true };
      return true;
    }
    ElNotification({
      title: "刷新token失败",
      message: `${response.message} code: ${response.code}`,
      type: "error",
      duration: 3000
    });
    return { path: LOGIN_URL, replace: true };
  }
  const menuStore = useMenuStore();
  if (!menuStore.hasMenuRoutes) {
    if (to.path == SWITCH_ROLE_URL) return true;
    if (userStore.isMultiRoles) return { path: SWITCH_ROLE_URL, replace: true };

    const response = await getMyRoutes();
    if (!response.isSuccess) {
      ElNotification({
        title: "加载菜单失败",
        message: `加载菜单失败，${response.message}，code:${response.code}`,
        type: "error"
      });
    }

    //需要转到选择角色页面 code=9
    if (response.code > 0) return { path: SWITCH_ROLE_URL, replace: true };

    const menuRoutes = response.data as IMenuRoute[];
    menuStore.setState(menuRoutes);
  }

  //token有效，跳过登录，地址栏输入地址，直接进入对应页面，此时没有菜单需要生成
  if (router.getRoutes().length <= staticRouter.length) {
    menuStore.loadAsyncMenuRoutes();
    return { path: to.fullPath, replace: true };
  }
  return true;
});
/**
 * @description 重置路由
 * */
export const resetRouter = () => {
  const menuStore = useMenuStore();
  menuStore.flatMenuRoutes.forEach(route => {
    const { name } = route;
    if (name && router.hasRoute(name)) router.removeRoute(name);
  });
};
/**
 * @description 路由跳转错误
 * */
router.onError(error => {
  NProgress.done();
  console.warn("路由错误", error.message);
});

/**
 * @description 路由跳转结束
 * */
router.afterEach(() => {
  NProgress.done();
});

export default router;
