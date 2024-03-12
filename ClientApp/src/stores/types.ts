export type LayoutType = "vertical" | "classic" | "transverse" | "columns";

export type AssemblySizeType = "large" | "default" | "small";

export type LanguageType = "zh" | "en" | null;

/* IGlobalState */
export interface IGlobalState {
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
export interface IUserState {
  userId: string;
  userName: string;
  accessToken: string;
  refreshToken: string;
  expires: number;
  roles: string;
}

/* tabsMenuProps */
export interface ITabPageState {
  icon: string;
  title: string;
  path: string;
  name: string;
  close: boolean;
  isKeepAlive: boolean;
}

export interface IMenuRoute {
  path: string;
  name: string;
  component?: string | (() => Promise<unknown>);
  redirect?: string;
  meta: IMenuRouteMeta;
  children?: IMenuRoute[];
}
export interface IMenuRouteMeta {
  title: string;
  icon: string;
  //1:menu item, 2:leaf menu item, 3:component
  routeType: number;
  linkUrl?: string;
  isHidden: boolean;
  isFull: boolean;
  isAffix: boolean;
  menuPath: string;
  isKeepAlive: boolean;
}

/* KeepAliveState */
export interface KeepAliveState {
  keepAliveNames: string[];
}
