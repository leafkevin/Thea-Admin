import axios, {
  AxiosInstance,
  AxiosError,
  AxiosRequestConfig,
  InternalAxiosRequestConfig,
  AxiosResponse,
  CustomParamsSerializer
} from "axios";
import { tryHideFullScreenLoading } from "@/components/Loading/fullScreen";
import { ElMessage } from "element-plus";
import { useUserStore } from "@/stores/account";
import router from "@/routers";
import { stringify } from "qs";
import { IResponse } from "./types";

const config = {
  // 默认地址请求地址，可在 .env.** 文件中修改
  baseURL: import.meta.env.VITE_API_URL as string,
  // 设置超时时间
  timeout: 30000,
  // 跨域时候允许携带凭证
  withCredentials: true,
  headers: {
    Accept: "application/json, text/plain, */*",
    "Content-Type": "application/json",
    "X-Requested-With": "XMLHttpRequest"
  },
  // 数组格式参数序列化（https://github.com/axios/axios/issues/5142）
  paramsSerializer: {
    serialize: stringify as unknown as CustomParamsSerializer
  }
};

class RequestHttp {
  service: AxiosInstance;
  /** token过期后，暂存待执行的请求 */
  private static requests: any[] = [];

  public constructor(config: AxiosRequestConfig) {
    // instantiation
    this.service = axios.create(config);
    this.setRequestInterceptors();
    this.setResponseInterceptors();
  }

  /** 格式化token（jwt格式） */
  formatToken(token: string | undefined): string {
    return "Bearer " + token;
  }
  /** 请求拦截 */
  private setRequestInterceptors(): void {
    this.service.interceptors.request.use(
      (config: InternalAxiosRequestConfig) => {
        const userStore = useUserStore();
        if (userStore.isAuthorized) {
          config.headers["Authorization"] = this.formatToken(userStore.accessToken);
        }
        return config;
      },
      (error: AxiosError) => {
        return Promise.reject(error);
      }
    );
  }

  /** 响应拦截 */
  private setResponseInterceptors(): void {
    this.service.interceptors.response.use(
      (response: AxiosResponse) => {
        const { status, data } = response;
        tryHideFullScreenLoading();
        const theaResponse = data as IResponse;
        // 登录失效
        if (status == 401) {
          // router.replace(LOGIN_URL);
          ElMessage.error(theaResponse.message);
          return Promise.reject(theaResponse);
        }
        // 全局错误信息拦截（防止下载文件的时候返回数据流，没有 code 直接报错）
        if (!theaResponse.isSuccess) {
          ElMessage.error(theaResponse.message);
          return Promise.reject(data);
        }
        return response.data;
      },
      async (error: AxiosError) => {
        const { response } = error;
        tryHideFullScreenLoading();
        // 请求超时 && 网络错误单独判断，没有 response
        // if (error.message.indexOf("timeout") !== -1) ElMessage.error("请求超时！请您稍后重试");
        // if (error.message.indexOf("Network Error") !== -1) ElMessage.error("网络错误！请您稍后重试");
        // 根据服务器响应的错误状态码，做不同的处理
        if (response) {
          switch (response.status) {
            case 400:
              ElMessage.error("请求失败！请您稍后重试");
              break;
            case 401:
              ElMessage.error("认证失效！请您重新登录");
              break;
            case 403:
              ElMessage.error("当前账号无权限访问！");
              break;
            case 404:
              ElMessage.error("你所访问的资源不存在！");
              break;
            case 405:
              ElMessage.error("请求方式错误！请您稍后重试");
              break;
            case 408:
              ElMessage.error("请求超时！请您稍后重试");
              break;
            case 500:
              ElMessage.error("服务异常！");
              break;
            case 502:
              ElMessage.error("网关错误！");
              break;
            case 503:
              ElMessage.error("服务不可用！");
              break;
            case 504:
              ElMessage.error("网关超时！");
              break;
            default:
              ElMessage.error("请求失败！");
          }
        }
        // 服务器结果都没有返回(可能服务器错误可能客户端断网)，断网处理:可以跳转到断网页面
        if (!window.navigator.onLine) router.replace("/500");
        return Promise.reject(error);
      }
    );
  }
  /**
   * @description 常用请求方法封装
   */
  get(url: string, params?: object): Promise<IResponse> {
    return this.service.get(url, params);
  }
  post(url: string, params?: object): Promise<IResponse> {
    return this.service.post(url, params);
  }
  put(url: string, params?: object): Promise<IResponse> {
    return this.service.put(url, params);
  }
  delete(url: string, params?: object): Promise<IResponse> {
    return this.service.delete(url, params);
  }
  download(url: string, params?: object): Promise<BlobPart> {
    return this.service.post(url, params, { responseType: "blob" });
  }
}

export const http = new RequestHttp(config);
