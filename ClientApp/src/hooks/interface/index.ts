export interface IPagingState {
  pageNumber: number;
  pageSize: number;
  total: number;
}
export interface ITableState {
  tableData: any[];
  pagination: IPagingState;
  searchParameters: {
    [key: string]: any;
  };
  searchInitParameters: {
    [key: string]: any;
  };
  totalParameters: {
    [key: string]: any;
  };
  icon?: {
    [key: string]: any;
  };
}
export interface IPagingData {
  totalCount: number;
  pageIndex: number;
  pageSize: number;
  data: any[];
}

export namespace HandleData {
  export type MessageType = "" | "success" | "warning" | "info" | "error";
}

export namespace Theme {
  export type ThemeType = "light" | "inverted" | "dark";
  export type GreyOrWeakType = "grey" | "weak";
}
