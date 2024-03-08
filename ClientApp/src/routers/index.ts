import { createRouter, createWebHashHistory, createWebHistory } from "vue-router";
import { refreshToken } from "@/api/account";
import { useUserStore } from "@/stores/account";
import { LOGIN_URL, SWITCH_ROLE_URL } from "@/config";
import { staticRouter, errorRouter } from "@/routers/staticRouter";
import NProgress from "@/utils/nprogress";
import { ElNotification } from "element-plus";

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
  routes: [...staticRouter, ...errorRouter],
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

  // 4.判断是否登陆页面，直接放行
  if (to.path == LOGIN_URL) return true;

  // 5.判断是否有 Token，没有重定向到 login 页面
  if (!userStore.isAuthorized) return { name: "Login", replace: true };

  // token已过期，调用刷新token接口
  if (userStore.isExpired) {
    const resp = await refreshToken({ refreshToken: userStore.refreshToken });
    const response = resp.data;
    if (response.isSuccess) {
      userStore.setState(response.data as IUserState);
      //code=9 跳转到切换角色页面
      if (response.code > 0) return { name: "switchRole", replace: true };
      return true;
    }
    ElNotification({
      title: "刷新token失败",
      message: `${response.message} code: ${response.code}`,
      type: "error",
      duration: 3000
    });
    return { name: "Login", replace: true };
  }
  if (!userStore.hasMenuRoutes && to.path !== SWITCH_ROLE_URL) return { name: "switchRole", replace: true };
  return true;
});

/**
 * @description 重置路由
 * */
export const resetRouter = () => {
  const userStore = useUserStore();
  userStore.flatMenuRoutes.forEach(route => {
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
