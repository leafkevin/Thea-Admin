import { http } from "./http";

export interface IDepositState {
  memberId: string;
  memberName: string;
  mobile: string;
  balance: number | string;
  amount: number | string;
  bonus: number | string;
  depositedAt?: Date;
  description: string;
}
export const queryPage = (parameters: object) => {
  return http.post("/deposit/queryPage", parameters);
};
export const createDeposit = (parameters: object) => {
  return http.post("/deposit/create", parameters);
};
export const exportDeposits = (parameters: object) => {
  return http.download("/deposit/export", parameters);
};
