export interface IResponse {
  isSuccess: boolean;
  code: number;
  message?: string;
  data?: any;
}
