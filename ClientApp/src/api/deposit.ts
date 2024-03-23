import { http } from "./http";

export interface IDepositState {
  depositId?: string;
  memberId: string;
  memberName: string;
  mobile: string;
  beginBalance: number | string;
  amount: number | string;
  bonus: number | string;
  endBalance: number | string;
  depositedAt?: Date;
  description: string;
}
export const queryPage = (parameters: object) => {
  return http.post("/deposit/queryPage", parameters);
};
export const getDeposit = (id: string) => {
  return http.get(`/deposit/detail?id=${id}`);
};
export const createDeposit = (parameters: object) => {
  return http.post("/deposit/create", parameters);
};
export const modifyDeposit = (parameters: object) => {
  return http.post("/deposit/modify", parameters);
};
export const cancelDeposit = (parameters: object) => {
  return http.post("/deposit/cancel", parameters);
};
export const exportDeposits = (parameters: object) => {
  return http.download("/deposit/export", parameters);
};
