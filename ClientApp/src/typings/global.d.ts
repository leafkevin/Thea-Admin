/* FileType */
declare namespace File {
  type ImageMimeType =
    | "image/apng"
    | "image/bmp"
    | "image/gif"
    | "image/jpeg"
    | "image/pjpeg"
    | "image/png"
    | "image/svg+xml"
    | "image/tiff"
    | "image/webp"
    | "image/x-icon";

  type ExcelMimeType = "application/vnd.ms-excel" | "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
}

/* Vite */
declare type Recordable<T = any> = Record<string, T>;

declare interface ViteEnv {
  VITE_USER_NODE_ENV: "development" | "production" | "test";
  VITE_GLOB_APP_TITLE: string;
  VITE_PORT: number;
  VITE_OPEN: boolean;
  VITE_REPORT: boolean;
  VITE_ROUTER_MODE: "hash" | "history";
  VITE_BUILD_COMPRESS: "gzip" | "brotli" | "gzip,brotli" | "none";
  VITE_BUILD_COMPRESS_DELETE_ORIGIN_FILE: boolean;
  VITE_DROP_CONSOLE: boolean;
  VITE_PWA: boolean;
  VITE_PUBLIC_PATH: string;
  VITE_API_URL: string;
  VITE_PROXY: [string, string][];
}

interface ImportMetaEnv extends ViteEnv {
  __: unknown;
}

/* __APP_INFO__ */
declare const __APP_INFO__: {
  pkg: {
    name: string;
    version: string;
    dependencies: Recordable<string>;
    devDependencies: Recordable<string>;
  };
  lastBuildTime: string;
};

declare type LayoutType = "vertical" | "classic" | "transverse" | "columns";

declare type AssemblySizeType = "large" | "default" | "small";

declare type LanguageType = "zh" | "en" | null;

/* IGlobalState */
declare interface IGlobalState {
  layout: LayoutType;
  assemblySize: AssemblySizeType;
  language: LanguageType;
  maximize: boolean;
  primary: string;
  isDark: boolean;
  isGrey: boolean;
  isWeak: boolean;
  asideInverted: boolean;
  headerInverted: boolean;
  isCollapse: boolean;
  accordion: boolean;
  breadcrumb: boolean;
  breadcrumbIcon: boolean;
  tabs: boolean;
  tabsIcon: boolean;
  footer: boolean;
}

/* IUserState */
declare interface IUserState {
  userId: string;
  userName: string;
  accessToken: string;
  refreshToken: string;
  expires: number;
  roles: string;
  menuRoutes: IMenuRoute[];
}

/* tabsMenuProps */
declare interface ITabPageState {
  icon: string;
  title: string;
  path: string;
  name: string;
  close: boolean;
  isKeepAlive: boolean;
}

declare interface IMenuRoute {
  path: string;
  name: string;
  component?: string | (() => Promise<unknown>);
  redirect?: string;
  meta: IMenuRouteMeta;
  children?: IMenuRoute[];
}
declare interface IMenuRouteMeta {
  title: string;
  icon: string;
  linkUrl?: string;
  isHidden: boolean;
  isFull: boolean;
  isAffix: boolean;
  menuPath: string;
  isKeepAlive: boolean;
}

/* KeepAliveState */
declare interface KeepAliveState {
  keepAliveNames: string[];
}
